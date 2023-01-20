using System;
using InfoTrackSEO.Contexts;
using InfoTrackSEO.Providers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

[assembly: FunctionsStartup(typeof(InfoTrackSEO.Functions.Startup))]

namespace InfoTrackSEO.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            SecretProviders.LoadSecrets();

            // Inject the DbContext
            builder.Services.AddDbContext<InfoTrackContext>();

            // Add Logging
            //var loggingConnectionString = Environment.GetEnvironmentVariable("LOGGING_CONNECTION_STRING");
            //if (!string.IsNullOrEmpty(loggingConnectionString))
            //{
            //    var logger = new LoggerConfiguration()
            //        .WriteTo.AzureTableStorage(loggingConnectionString, storageTableName: "logs")
            //        .CreateLogger();

            //    builder.Services.AddSingleton<ILoggerProvider>(sp => new SerilogLoggerProvider(logger, true));
            //}
        }
    }
}