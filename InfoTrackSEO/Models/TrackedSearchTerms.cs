using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfoTrackSEO.Models;

public class TrackedSearchTerms
{
    public int Id { get; set; }

    public Guid UUID { get; set; } = Guid.NewGuid();
    
    public int UrlId { get; set; }

    public TrackedUrls Url { get; set; }

    public string Term { get; set; }

    public string Source { get; set; }

    public ICollection<TrackedSearchData>? SearchData { get; set; }
}