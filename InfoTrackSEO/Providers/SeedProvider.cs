using InfoTrackSEO.Models;

namespace InfoTrackSEO.Providers;

public static class SeedProvider
{
    public static SeedData GetSeedData()
    {
        var seedData = new SeedData
        {
            Urls = new List<TrackedUrls> {new() { Id = 1, Url = "https://www.infotrack.com"}, new() { Id = 2, Url = "https://www.onelegal.com" } },
            Terms = new List<TrackedSearchTerms> {new() { Id = 1, Term = "efiling integration", UrlId = 1 }, new() { Id = 2, Term = "efiling integration", UrlId = 2 } }, 
            SearchData = new List<TrackedSearchData>()
        };

        return seedData;
    }
}