using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace YAMDB.Models;

public class MovieRatings
{
    public int Id { get; set; }

    [ForeignKey("Movie")]
    [Display(Name = "Movie")]
    public int MovieId { get; set; }

    [JsonIgnore] public Movies? Movie { get; set; }

    public string? Source { get; set; }

    public double Rating { get; set; }

    public double RatingUpperLimit { get; set; }

    public int TotalReviews { get; set; }
}