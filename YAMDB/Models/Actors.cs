using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace YAMDB.Models;

[Index(nameof(UUID), IsUnique = true)]
[Index(nameof(Name))]
[Index(nameof(TheMovieDbId), IsUnique = true)]
public class Actors
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("uuid")] public Guid UUID { get; set; } = Guid.NewGuid();

    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("theMovieDbId")] public int TheMovieDbId { get; set; }

    [JsonPropertyName("movies")] public ICollection<Movies>? Movies { get; set; }

    [JsonIgnore] public List<ActorsInMovies>? ActorsInMovies { get; set; }
}