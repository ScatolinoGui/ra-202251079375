namespace EstoquePedidos.Api.DTOs;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();

    public long Total { get; init; }

    public int Page { get; init; }

    public int PageSize { get; init; }

    public PagedResult()
    {
    }

    public PagedResult(IReadOnlyList<T> items, long total, int page, int pageSize)
    {
        Items = items;
        Total = total;
        Page = page;
        PageSize = pageSize;
    }
}
