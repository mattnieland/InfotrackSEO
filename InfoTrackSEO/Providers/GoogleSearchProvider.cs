using System.Text.RegularExpressions;
using InfoTrackSEO.Models;

namespace InfoTrackSEO.Providers;

public class GoogleSearchProvider : ISearchProvider
{
    private static readonly string baseUrl = "https://www.google.com/search";

    private static readonly string userAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36 Edg/109.0.1518.55";

    private readonly HttpClient client = new();

    private readonly RegexOptions options = RegexOptions.Multiline;

    public string GetSource()
    {
        return "Google";
    }

    public async Task<List<SearchResults>> GetSearchResults(string searchQuery, int limit = 100)
    {
        var results = new List<SearchResults>();

        try
        {
            var blocks = await GetSearchBlocks(searchQuery, limit);
            var resultIndex = 1;
            foreach (var block in blocks)
            {
                var link = Regex.Matches(block, @"<a\s+(?:[^>]*?\s+)?href=([""])(.*?)\1", options).FirstOrDefault();
                if (link == null)
                {
                    throw new Exception("No link found in search block");
                }

                var header = Regex.Matches(block, @"<h3\b[^>]*>(.|\n)*?<\/h3>", options).FirstOrDefault();
                if (header == null)
                {
                    throw new Exception("No header found in search block");
                }

                var result = new SearchResults
                {
                    Title = header.Value.Substring(header.Value.IndexOf('>') + 1,
                        header.Value.LastIndexOf('<') - header.Value.IndexOf('>') - 1),
                    Url = link.Groups[2].Value,
                    Rank = resultIndex
                };
                results.Add(result);
                resultIndex++;
            }

            return results;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task<List<string>> GetSearchBlocks(string searchQuery, int limit = 100)
    {
        var blocks = new List<string>();

        try
        {
            client.DefaultRequestHeaders.Add("User-Agent", userAgent);
            var response = await client.GetAsync($"{baseUrl}?q={searchQuery}&num={limit}");
            var contents = await response.Content.ReadAsStringAsync();
            var blockStart = @"<div\b[^>]*data-sokoban-container\b[^>]*>";
            var blockMatches = Regex.Matches(contents, blockStart, options);
            foreach (Match block in blockMatches)
            {
                var start = block.Index;
                var end = contents.IndexOf("</div>", start, StringComparison.Ordinal);
                var blockText = contents.Substring(start, end - start + 6);
                var openDivCount = Regex.Matches(blockText, "<div").Count;
                var closeDivCount = Regex.Matches(blockText, "</div>").Count;
                var newClosingPosition = end;

                // keep going until you reach the actual end tag
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
        catch (Exception)
        {
            throw;
        }
    }
}