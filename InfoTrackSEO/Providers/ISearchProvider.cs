using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoTrackSEO.Models;

namespace InfoTrackSEO.Providers
{
    public interface ISearchProvider
    {
        Task<List<SearchResults>> GetSearchResults(string searchQuery, int limit = 100);

    }
}
