namespace EmployeePortal.Models.DTOs;

public class PagedResultDto<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 1;
}

public class SearchQueryDto
{
    public string? SearchTerm { get; set; }
    public int? DepartmentId { get; set; }
    public string? StatusFilter { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "LastName";
    public bool SortDescending { get; set; }
}

