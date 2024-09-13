namespace IMS.SharedKernel;

public sealed record Page<T> where T : class
{
    public int Cursor { get; init; }
    public int PageSize { get; init; }
    public IEnumerable<T> Values { get; init; }
    public bool IsLastPage { get; init; } = false;
    public bool IsFirstPage { get; init; } = true;

    public Page(int cursor, int pageSize, IEnumerable<T> values)
    {
        Cursor = cursor;
        PageSize = pageSize;
        Values = values;
    }
}
