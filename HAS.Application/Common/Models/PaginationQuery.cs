namespace HAS.Application.Common.Models;

public record PaginationQuery
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public PaginationQuery()
    {
    }

    public PaginationQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);
    }
}
