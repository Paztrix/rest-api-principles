using WebApplication.todo.dto;
using WebApplication.todo.exceptions;
using WebApplication.todo.models;
using WebApplication.todo.repositories;

namespace WebApplication.todo.services;

public class TodoService(ITodoRepository todoRepository)
{
    private readonly ITodoRepository _todoRepository = todoRepository;

    // -------------------------------------------------------------------------
    // TODO 1 — FindAllAsync
    // -------------------------------------------------------------------------
    // If `status` is not null, filter todos by that status.
    // Otherwise return all todos.
    // Map each TodoModel entity to a TodoDto using ToDto().
    // -------------------------------------------------------------------------
    public async Task<PagedResponse<TodoDto>> FindAllAsync(string? status, int page, int size, CancellationToken cancellationToken = default)
    {
        if (page < 0) 
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be zero or greater");
        }

        if (size < 0) 
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Size must be greate than 1");
        }

        TodoStatus? todoStatus = status == null
            ? null
            : Enum.Parse<TodoStatus>(status, ignoreCase: true);

        var totalCount = await _todoRepository.CountAsync(todoStatus, cancellationToken);

        var todos = await _todoRepository.FindPageAsync(
            todoStatus,
            page,
            size,
            cancellationToken);

        return new PagedResponse<TodoDto>(
            todos.Select(ToDto).ToList(),
            page,
            size,
            totalCount,
            (int)Math.Ceiling(totalCount / (double)size));
    }

    // -------------------------------------------------------------------------
    // TODO 2 — FindByIdAsync
    // -------------------------------------------------------------------------
    // Find a TodoModel by id. If not found, throw ResourceNotFoundException.ForTodo(id).
    // Return the result mapped to a TodoDto.
    // -------------------------------------------------------------------------
    public async Task<TodoDto> FindByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var todo = await _todoRepository.FindByIdAsync(id, cancellationToken)
            ?? throw ResourceNotFoundException.ForTodo(id);

        return ToDto(todo);
    }

    // -------------------------------------------------------------------------
    // TODO 3 — CreateAsync
    // -------------------------------------------------------------------------
    // Build a new TodoModel entity from the request, save it, and return the DTO.
    // -------------------------------------------------------------------------
    public async Task<TodoDto> CreateAsync(CreateTodoRequest request, CancellationToken cancellationToken = default)
    {
        var todo = new TodoModel 
        {
            Title = request.Title!,
            Description = request.Description,
            DueDate = request.DueDate
        };

        var savedTodo = await _todoRepository.SaveAsync(todo, cancellationToken);
        return ToDto(savedTodo);
    }

    // -------------------------------------------------------------------------
    // TODO 4 — UpdateAsync
    // -------------------------------------------------------------------------
    // Find the todo by id (throw 404 if missing). Apply non-null fields from
    // the request. Save and return the updated DTO.
    // -------------------------------------------------------------------------
    public async Task<TodoDto> UpdateAsync(long id, UpdateTodoRequest request, CancellationToken cancellationToken = default)
    {
        var todo = await _todoRepository.FindByIdAsync(id, cancellationToken)
            ?? throw ResourceNotFoundException.ForTodo(id);

        if (request.Title != null) todo.Title = request.Title;
        if (request.Description != null) todo.Description = request.Description;
        if (request.Status != null) todo.Status = request.Status.Value;
        if (request.DueDate != null) todo.DueDate = request.DueDate;

        var savedTodo = await _todoRepository.SaveAsync(todo, cancellationToken);
        return ToDto(savedTodo);
    }

    // -------------------------------------------------------------------------
    // TODO 5 — UpdateStatusAsync
    // -------------------------------------------------------------------------
    // Find the todo by id (throw 404 if missing). Set its status. Return DTO.
    // -------------------------------------------------------------------------
    public async Task<TodoDto> UpdateStatusAsync(
        long id,
        TodoStatus newStatus,
        CancellationToken cancellationToken = default)
    {
        var todo = await _todoRepository.FindByIdAsync(id, cancellationToken)
            ?? throw ResourceNotFoundException.ForTodo(id);

        todo.Status = newStatus;

        var savedTodo = await _todoRepository.SaveAsync(todo, cancellationToken);
        return ToDto(savedTodo);
    }

    // -------------------------------------------------------------------------
    // TODO 6 — DeleteAsync
    // -------------------------------------------------------------------------
    // Find the todo by id (throw 404 if missing). Then delete it.
    // -------------------------------------------------------------------------
    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var todo = await _todoRepository.FindByIdAsync(id, cancellationToken)
            ?? throw ResourceNotFoundException.ForTodo(id);

        await _todoRepository.DeleteAsync(todo, cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Mapping helper — converts a TodoModel entity to a TodoDto.
    // You can use this in all methods above with: todos.Select(ToDto).
    // -------------------------------------------------------------------------
    private static TodoDto ToDto(TodoModel todo) => new()
    {
        Id = todo.Id,
        Title = todo.Title,
        Description = todo.Description,
        Status = todo.Status.ToString(),
        CreatedAt = todo.CreatedAt,
        DueDate = todo.DueDate
    };
}
