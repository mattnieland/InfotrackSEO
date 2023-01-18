using System.Text.Json;
using RestSharp;
using YAMDB.Models.TheMovieDb;

namespace YAMDB.Providers;

/// <summary>
///     A provider class for TheMovieDb
/// </summary>
public class TMDBProvider
{
    private static readonly string baseUrl = "https://api.themoviedb.org/3";
    private readonly RestClient client = new(baseUrl);

    public async Task<List<Genre>> GetGenres()
    {
        try
        {
            if (!SecretProviders.SecretsLoaded)
            {
                SecretProviders.LoadSecrets();
            }
            else if (Environment.GetEnvironmentVariable("THEMOVIEDB_API_KEY") == null)
            {
                throw new Exception("Missing API key for TheMovieDb.");
            }

            var endpoint =
                $"{baseUrl}/genre/movie/list?api_key={Environment.GetEnvironmentVariable("THEMOVIEDB_API_KEY")}&language=en-US";
            var request = new RestRequest(endpoint);
            var response = await client.ExecuteAsync(request);
            if (string.IsNullOrEmpty(response.Content))
            {
                throw new Exception("Response content is empty.");
            }

            var result = JsonSerializer.Deserialize<MovieGenreResponse>(response.Content);
            if (result == null)
            {
                throw new Exception("Failed to deserialize response.");
            }

            return result.Genres;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<Cast>> GetMovieCast(int movieId)
    {
        try
        {
            if (!SecretProviders.SecretsLoaded)
            {
                throw new Exception("Configuration is null");
            }
            else if (Environment.GetEnvironmentVariable("THEMOVIEDB_API_KEY") == null)
            {
                throw new Exception("Missing API key for TheMovieDb.");
            }

            var endpoint =
                $"{baseUrl}/movie/{movieId}/credits?api_key={Environment.GetEnvironmentVariable("THEMOVIEDB_API_KEY")}&language=en-US";
            var request = new RestRequest(endpoint);
            var response = await client.ExecuteAsync(request);
            if (string.IsNullOrEmpty(response.Content))
            {
                throw new Exception("Response content is empty.");
            }

            var result = JsonSerializer.Deserialize<MovieCastResponse>(response.Content);
            if (result == null)
            {
                throw new Exception("Failed to deserialize response.");
            }

            return result.Cast;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<TopMovie>> GetTopMovies(int? page = null)
    {
        var movies = new List<TopMovie>();
        try
        {
            var genres = (await GetGenres()).ToDictionary(k => k.Id, v => v.Name ?? string.Empty);
            if (!SecretProviders.SecretsLoaded)
            {
                throw new Exception("Configuration is null");
            }
            else if (Environment.GetEnvironmentVariable("THEMOVIEDB_API_KEY") == null)
            {
                throw new Exception("Missing API key for TheMovieDb.");
            }

            var endpoint =
                $"{baseUrl}/movie/top_rated?api_key={Environment.GetEnvironmentVariable("THEMOVIEDB_API_KEY")}&language=en-US&page={page ?? 1}";
            var request = new RestRequest(endpoint);
            var response = await client.ExecuteAsync(request);
            if (string.IsNullOrEmpty(response.Content))
            {
                return movies;
            }

            var result = JsonSerializer.Deserialize<TopRatedMoviesResponse>(response.Content);
            if (result?.Results == null)
            {
                return movies;
            }

            movies.AddRange(result.Results);

            #region Load More Pages

            if (page == null)
            {
                Parallel.For(2, 50, pg =>
                {
                    endpoint =
                        $"{baseUrl}/movie/top_rated?api_key={Environment.GetEnvironmentVariable("THEMOVIEDB_API_KEY")}&language=en-US&page={pg}";
                    request = new RestRequest(endpoint);
                    response = client.ExecuteAsync(request).Result;
                    if (string.IsNullOrEmpty(response.Content))
                    {
                        return;
                    }

                    result = JsonSerializer.Deserialize<TopRatedMoviesResponse>(response.Content);
                    if (result?.Results == null)
                    {
                        return;
                    }

                    result.Results.RemoveAll(m => m.Id == null);
                    movies.AddRange(result.Results);
                });
            }

            #endregion

            return movies.OrderBy(m => m.Id).ToList();
        }
        catch (Exception)
        {
            throw;
        }
    }
}