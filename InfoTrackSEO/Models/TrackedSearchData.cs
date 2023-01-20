using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfoTrackSEO.Models;

public class TrackedSearchData
{
    [Key]
    public int Id { get; set; }

    public Guid UUID { get; set; } = Guid.NewGuid();

    public int UrlId { get; set; }

    public TrackedUrls Url { get; set; }

    public int TermId { get; set; }

    public TrackedSearchTerms SearchTerms { get; set; }

    // I would've liked to use List<int> here, but
    // EF Core complains about lists of primitive types
    // So I'll just split the csv string
    // in the front end for now
    public string? Ranks { get; set; }

    public string Source { get; set; }

    public int CaptureSize { get; set; } = 100;

    public DateTime SearchDate { get; set; } = DateTime.Now;
}