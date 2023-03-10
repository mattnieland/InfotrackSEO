using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using InfoTrackSEO.Api.Authentication;
using InfoTrackSEO.Api.Extensions;
using InfoTrackSEO.Api.Repositories;
using InfoTrackSEO.Contexts;
using InfoTrackSEO.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Load the secrets from our provider
SecretProviders.LoadSecrets();

// default to lowercase URLs
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Add Gzip compression to responses
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
});

// Inject the DbContext
builder.Services.AddDbContext<InfoTrackContext>();

// Add our repositories
builder.Services.AddScoped(typeof(IUrlsRepository), typeof(UrlsRepository));
builder.Services.AddScoped(typeof(ITermsRepository), typeof(TermsRepository));
builder.Services.AddScoped(typeof(ISearchDataRepository), typeof(SearchDataRepository));

// Add Logging
var loggingConnectionString = Environment.GetEnvironmentVariable("LOGGING_CONNECTION_STRING");
if (!string.IsNullOrEmpty(loggingConnectionString))
{
    var logger = new LoggerConfiguration()
        .WriteTo.AzureTableStorage(loggingConnectionString, storageTableName: "logs")
        .CreateLogger();

    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(logger);
}

// Add Sentry
var dsn = Environment.GetEnvironmentVariable("SENTRY_DSN");
if (!string.IsNullOrEmpty(dsn))
{
    builder.WebHost.UseSentry(o =>
    {
        o.Dsn = dsn;
        o.Debug = true;
        o.TracesSampleRate = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ? 1.0 : .5;
    });
}

// default to indented and camel casing JSON
// ignore looped references
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

#region Add Authorization (via Auth0)

var domain = Environment.GetEnvironmentVariable("AUTH0_AUTHORITY");
if (string.IsNullOrEmpty(domain))
{
    throw new Exception("Auth0 domain is not set");
}

var audience = Environment.GetEnvironmentVariable("AUTH0_AUDIENCE") ?? "YOUR_API_IDENTIFIER";

// authenticate with JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = domain;
        options.Audience = audience;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

// add scope policies
builder.Services
    .AddAuthorization(options =>
    {
        options.AddPolicy(
            "write:urls",
            policy => policy.Requirements.Add(
                new HasScopeRequirement("write:urls", domain)
            )
        );

        options.AddPolicy(
            "read:urls",
            policy => policy.Requirements.Add(
                new HasScopeRequirement("read:urls", domain)
            )
        );

        options.AddPolicy(
            "write:terms",
            policy => policy.Requirements.Add(
                new HasScopeRequirement("write:terms", domain)
            )
        );

        options.AddPolicy(
            "read:terms",
            policy => policy.Requirements.Add(
                new HasScopeRequirement("read:terms", domain)
            )
        );

        options.AddPolicy(
            "read:data",
            policy => policy.Requirements.Add(
                new HasScopeRequirement("read:searches", domain)
            )
        );
    });

// Add scope handler
builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

#endregion

builder.Services.AddDistributedMemoryCache();

#region Add Swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    // Use XML comments in documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    opt.IncludeXmlComments(xmlPath, true);

    // Use annotations in documentation
    opt.EnableAnnotations();

    opt.SwaggerDoc("v1", new OpenApiInfo {Title = "InfoTrackSEO Api", Version = "v1"});

    // Add JWT authentication to the Swagger UI
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

#endregion

var app = builder.Build();
app.UseSentryTracing();

#region Ensure database is created (this will seed the data if in Development)

var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetService<InfoTrackContext>();
if (db == null)
{
    throw new Exception("Could not get database context");
}

//db.Database.EnsureDeleted();
db.Database.EnsureCreated();

#endregion

#region Set up branded Swagger files

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Content")),
    RequestPath = "/Content"
});
app.UseSwagger();
app.UseSwaggerUI(setupAction => { setupAction.InjectStylesheet("/Content/css/swagger-custom.css"); });

#endregion

#region Exception Handling (for Production)

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = Text.Plain;

            await context.Response.WriteAsync("An exception was thrown.");

            var exceptionHandlerPathFeature =
                context.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
            {
                await context.Response.WriteAsync(" The file was not found.");
            }

            if (exceptionHandlerPathFeature?.Path == "/")
            {
                await context.Response.WriteAsync(" Page: Home.");
            }
        });
    });
}

#endregion

app.UseStatusCodePages();
app.UseResponseCompression();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Add Custom Rate Limiting middleware
app.UseRateLimiting();

app.Run();