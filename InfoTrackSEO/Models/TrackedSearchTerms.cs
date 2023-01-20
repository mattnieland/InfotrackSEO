using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfoTrackSEO.Models;

[Table("TrackedSearchTerms")]
public class TrackedSearchTerms
{
    [Key]
    public int Id { get; set; }

    public Guid UUID { get; set; } = Guid.NewGuid();

    public int UrlId { get; set; }

    public TrackedUrls Url { get; set; }

    public string Term { get; set; }

    public ICollection<TrackedSearchData>? SearchData { get; set; }
}