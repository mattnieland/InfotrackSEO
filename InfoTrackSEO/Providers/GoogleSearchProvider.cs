using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using InfoTrackSEO.Models;

namespace InfoTrackSEO.Providers
{
    public class GoogleSearchProvider : ISearchProvider
    {
        private static readonly string baseUrl = "https://www.google.com/search";
        private static readonly string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36 Edg/109.0.1518.55";
        private readonly HttpClient client = new();

        public Task<List<SearchResults>> GetSearchResults(string searchQuery, int limit = 100)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetSearchBlocks(string searchQuery, int limit = 100)
        {
            var blocks = new List<string>();

            try
            {
                client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                var response = await client.GetAsync($"{baseUrl}?q={searchQuery}&num={limit}");
                var contents = await response.Content.ReadAsStringAsync();
                var blockStart = @"<div\b[^>]*data-sokoban-container\b[^>]*>";
                var options = RegexOptions.Multiline;
                var blockMatches = Regex.Matches(contents, blockStart, options);
                foreach (Match block in blockMatches)
                {
                    var start = block.Index;
                    var end = contents.IndexOf("</div>", start, StringComparison.Ordinal);
                    var blockText = contents.Substring(start, end - start + 6);
                    var openDivCount = Regex.Matches(blockText, "<div").Count;
                    var closeDivCount = Regex.Matches(blockText, "</div>").Count;
                    var newClosingPosition = end;
                    while (openDivCount != closeDivCount)
                    {
                        var nextClosingDiv = contents.IndexOf("</div>", newClosingPosition, StringComparison.Ordinal);
                        newClosingPosition = nextClosingDiv + 6;
                        blockText = contents.Substring(start, newClosingPosition - start);
                        openDivCount = Regex.Matches(blockText, "<div").Count;
                        closeDivCount = Regex.Matches(blockText, "</div>").Count;
                    }
                    blocks.Add(blockText);
                }
                return blocks;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
