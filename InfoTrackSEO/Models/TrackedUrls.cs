namespace InfoTrackSEO.Models;

public class TrackedUrls
{
    public int Id { get; set; }

    public Guid UUID { get; set; } = Guid.NewGuid();

    public string Url { get; set; }

    public ICollection<TrackedSearchTerms>? SearchTerms { get; set; }

    public ICollection<TrackedSearchData>? SearchData { get; set; }
}