using System.Text.Json.Serialization;

namespace YAMDB.Models.TheMovieDb;

public class Genre
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("name")] public string? Name { get; set; }
}

public class MovieGenreResponse
{
    [JsonPropertyName("genres")] public List<Genre> Genres { get; set; } = new();
}