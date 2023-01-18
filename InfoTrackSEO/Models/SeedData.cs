#pragma warning disable CS8618
namespace InfoTrackSEO.Models;

public class SeedData
{
    public List<TrackedUrls> Urls { get; set; }
    public List<TrackedSearchTerms> Terms { get; set; }
    public List<TrackedSearchData> SearchData { get; set; }
}