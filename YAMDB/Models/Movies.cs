using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace YAMDB.Models;

[Index(nameof(UUID), IsUnique = true)]
[Index(nameof(Title))]
[Index(nameof(Genres))]
[Index(nameof(TheMovieDbId), IsUnique = true)]
public class Movies
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("uuid")] public Guid UUID { get; set; } = Guid.NewGuid();

    [JsonPropertyName("title")] public string? Title { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }

    [JsonPropertyName("genres")] public string? Genres { get; set; }

    [JsonPropertyName("imageUrl")] public string? ImageUrl { get; set; }

    [JsonPropertyName("releaseDate")] public DateTime? ReleaseDate { get; set; }

    [JsonPropertyName("theMovieDbId")] public int TheMovieDbId { get; set; }

    [JsonPropertyName("ratings")] public ICollection<MovieRatings>? Ratings { get; set; }

    [JsonPropertyName("actors")] public ICollection<Actors>? Actors { get; set; }

    [JsonIgnore] public List<ActorsInMovies>? ActorsInMovies { get; set; }
}