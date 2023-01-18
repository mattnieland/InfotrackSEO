using System.Text.Json.Serialization;

namespace YAMDB.Models;

public class Paging<T>
{
    [JsonPropertyName("page")] public int Page { get; set; }
    [JsonPropertyName("size")] public int Size { get; set; }
    [JsonPropertyName("morePages")] public bool MorePages { get; set; }
    [JsonPropertyName("total")] public int Total { get; set; }
    [JsonPropertyName("pages")] public int Pages { get; set; }
    [JsonPropertyName("results")] public IQueryable<T>? Results { get; set; }
}

public class Cursor
{
    [JsonPropertyName("after")] public Guid? After { get; set; }
    [JsonPropertyName("before")] public Guid? Before { get; set; }
    [JsonPropertyName("total")] public int Total { get; set; }
}

public class PagingCursor<T>
{
    [JsonPropertyName("results")] public IQueryable<T>? Results { get; set; }

    [JsonPropertyName("cursor")] public Cursor? Cursor { get; set; }
}