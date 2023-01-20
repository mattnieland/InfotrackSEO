using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfoTrackSEO.Models;

public class TrackedUrls
{
    [Key]
    public int Id { get; set; }

    public Guid UUID { get; set; } = Guid.NewGuid();

    public string Url { get; set; }

    public ICollection<TrackedSearchTerms>? SearchTerms { get; set; }

    public ICollection<TrackedSearchData>? SearchData { get; set; }
}