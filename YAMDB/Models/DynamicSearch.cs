#pragma warning disable CS8618
namespace YAMDB.Models;

public class Filter
{
    public string Field { get; set; }
    public string Operator { get; set; }

    public string Value { get; set; }
    //public string? Logic { get; set; }
    //public IEnumerable<Filter>? Filters { get; set; }
}

public class Sort
{
    public string Field { get; set; }
    public string Dir { get; set; }
}

public class DynamicSearch
{
    public int? Offset { get; set; }
    public int? Limit { get; set; }
    public IEnumerable<Sort>? Sort { get; set; }
    public Filter? Filter { get; set; }
}