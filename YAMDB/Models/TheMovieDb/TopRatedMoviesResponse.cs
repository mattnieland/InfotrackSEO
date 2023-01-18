using System.Text.Json.Serialization;

namespace YAMDB.Models.TheMovieDb;

public class TopMovie
{
    [JsonPropertyName("adult")] public bool Adult { get; set; }

    [JsonPropertyName("backdrop_path")] public string? BackdropPath { get; set; }

    [JsonPropertyName("genre_ids")] public List<int> GenreIds { get; set; } = new();

    [JsonPropertyName("id")] public int? Id { get; set; }

    [JsonPropertyName("original_language")]
    public string? OriginalLanguage { get; set; }

    [JsonPropertyName("original_title")] public string? OriginalTitle { get; set; }

    [JsonPropertyName("overview")] public string? Overview { get; set; }

    [JsonPropertyName("popularity")] public double Popularity { get; set; }

    [JsonPropertyName("poster_path")] public string? PosterPath { get; set; }

    [JsonPropertyName("release_date")] public string? ReleaseDate { get; set; }

    [JsonPropertyName("title")] public string? Title { get; set; }

    [JsonPropertyName("video")] public bool Video { get; set; }

    [JsonPropertyName("vote_average")] public double VoteAverage { get; set; }

    [JsonPropertyName("vote_count")] public int VoteCount { get; set; }
}

public class TopRatedMoviesResponse
{
    [JsonPropertyName("page")] public int Page { get; set; }

    [JsonPropertyName("results")] public List<TopMovie>? Results { get; set; } = new();

    [JsonPropertyName("total_pages")] public int TotalPages { get; set; }

    [JsonPropertyName("total_results")] public int TotalResults { get; set; }
}