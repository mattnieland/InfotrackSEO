using InfoTrackSEO.Models;

namespace InfoTrackSEO.Providers;

public interface ISearchProvider
{
    string GetSource();
    Task<List<SearchResults>> GetSearchResults(string searchQuery, int limit = 100);
}