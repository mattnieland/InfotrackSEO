using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace YAMDB.Providers;

/// <summary>
///     A provider class for injecting secrets into
///     Environment variables using Doppler
///     https://www.doppler.com
///     Mimics how secrets are injected in the front end
/// </summary>
public class SecretProviders
{
    private static readonly string baseUrl = "https://api.doppler.com/v3";
    private static readonly string project = "yamdb";
    private static readonly string config;
    private static readonly RestClient client = new(baseUrl);

    static SecretProviders()
    {
        config = GetConfig();
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{config}.json", true, true)
            .AddUserSecrets<SecretProviders>()
            .AddEnvironmentVariables();
        Configuration = builder.Build();
    }

    public static bool SecretsLoaded => Environment.GetEnvironmentVariable("DOPPLER_PROJECT") != null &&
                                        Environment.GetEnvironmentVariable("DOPPLER_CONFIG") != null &&
                                        Environment.GetEnvironmentVariable("DOPPLER_ENVIRONMENT") != null;

    private static IConfiguration? Configuration { get; set; }

    public static string GetConfig()
    {
#if LOCAL
        return "local";
#elif DEV
        return "dev";
#elif PROD
        return "prod";
#endif
    }

    public static void LoadSecrets()
    {
        if (SecretsLoaded)
        {
            return;
        }

        var endpoint =
            $"configs/config/secrets/download?project={project}&config={config}&format=json&include_dynamic_secrets=true&dynamic_secrets_ttl_sec=1800";
        var request = new RestRequest(endpoint);
        var token = Configuration!["DOPPLER_TOKEN"];
        if (string.IsNullOrEmpty(token))
        {
            throw new Exception("Doppler token not found");
        }

        request.AddHeader("Accept", "application/json");
        request.AddHeader("Authorization", $"Basic {token}");
        var response = client.Execute(request);
        if (response.Content != null)
        {
            var secretsDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(response.Content);
            if (secretsDictionary != null)
            {
                foreach (var secret in secretsDictionary)
                {
                    Environment.SetEnvironmentVariable(secret.Key, secret.Value);
                }
            }
        }
    }
}