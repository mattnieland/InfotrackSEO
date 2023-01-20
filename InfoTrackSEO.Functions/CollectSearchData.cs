using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoTrackSEO.Contexts;
using InfoTrackSEO.Models;
using InfoTrackSEO.Providers;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sentry;

namespace InfoTrackSEO.Functions;

public class CollectSearchData
{
    private readonly int captureSize = 100;
    private readonly List<ISearchProvider> providers;
    private readonly InfoTrackContext Context;

    public CollectSearchData(InfoTrackContext context)
    {
        Context = context;
        providers = new List<ISearchProvider>
        {
            new GoogleSearchProvider()
        };

        if (Environment.GetEnvironmentVariable("CAPTURE_SIZE") != null)
        {
            if (int.TryParse(Environment.GetEnvironmentVariable("CAPTURE_SIZE"), out var size))
            {
                captureSize = size;
            }
        }
    }

    [FunctionName("CollectSearchData")]
    public void Run([TimerTrigger("0 0 0 * * *")] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"Function executed at: {DateTime.Now}");
        using (SentrySdk.Init(o =>
               {
                   o.Dsn = Environment.GetEnvironmentVariable("SENTRY_DSN");
                   o.Debug = true;
                   o.TracesSampleRate = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                       ? 1.0
                       : .5;
               }))
        {
            try
            {
                var dataToAdd = GetSearchData(log, Context).Result;
                if (dataToAdd.Any())
                {
                    Context.TrackedSearchData.AddRange(dataToAdd);
                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.LogCritical(ex.Message, ex);
            }
        }

        log.LogInformation($"Function finished at: {DateTime.Now}");
    }

    private async Task<List<TrackedSearchData>> GetSearchData(ILogger log, InfoTrackContext context)
    {
        var dataToAdd = new List<TrackedSearchData>();
        var trackedUrls = context.TrackedUrls
            .Include(u => u.SearchTerms)
            .ToList();

        foreach (var url in trackedUrls)
        {
            var terms = url.SearchTerms.ToList();
            foreach (var term in terms)
            {
                foreach (var provider in providers)
                {
                    try
                    {
                        var results = await provider.GetSearchResults(term.Term, captureSize);
                        var matchingResults = results.Where(r =>
                            r.Url.Trim().StartsWith(url.Url.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
                        if (matchingResults.Any())
                        {
                            var data = new TrackedSearchData
                            {
                                TermId = term.Id,
                                UrlId = url.Id,
                                Ranks = string.Join(", ", matchingResults.Select(r => r.Rank).OrderBy(r => r)),
                                SearchDate = DateTime.Now.ToUniversalTime(),
                                Source = provider.GetSource()
                            };

                            dataToAdd.Add(data);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.LogCritical(ex.Message, ex);
                    }
                }
            }
        }

        return dataToAdd;
    }
}