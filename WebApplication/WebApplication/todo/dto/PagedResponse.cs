namespace WebApplication.todo.dto {
    public sealed record PagedResponse<T>(
        IReadOnlyList<T> Items,
        int Page,
        int Size,
        int TotalCount,
        int TotalPages);
}
