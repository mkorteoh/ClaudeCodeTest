namespace InsuranceDMS.Application.Common;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
    public PaginationMeta? Pagination { get; set; }

    public static ApiResponse<T> Success(T data, PaginationMeta? pagination = null) =>
        new() { Data = data, Pagination = pagination };

    public static ApiResponse<T> Failure(string error) =>
        new() { Errors = new List<string> { error } };

    public static ApiResponse<T> Failure(IEnumerable<string> errors) =>
        new() { Errors = errors.ToList() };
}

public class PaginationMeta
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
