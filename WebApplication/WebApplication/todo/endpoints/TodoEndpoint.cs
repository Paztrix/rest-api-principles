using Microsoft.AspNetCore.Mvc;
using WebApplication.todo.dto;
using WebApplication.todo.exceptions;
using WebApplication.todo.models;
using WebApplication.todo.services;

namespace WebApplication.todo.endpoints;

public static class TodoEndpoint
{
    public static RouteGroupBuilder MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var todos = app.MapGroup("/api/v1/todos");

        todos.MapGet(string.Empty, GetAllTodos)
            .WithName("GetTodos")
            .WithTags("Todos")
            .WithSummary("Lists todos")
            .WithDescription("Returns todos with optional status filtering and zero-based pagination")
            .Produces<PagedResponse<TodoDto>>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        todos.MapGet("/{id:long}", GetTodoById);

        todos.MapPost(string.Empty, CreateTodo);

        todos.MapPut("/{id:long}", UpdateTodo);

        todos.MapMethods("/{id:long}/status", ["PATCH"], UpdateStatus);

        todos.MapDelete("/{id:long}", DeleteTodo);

        return todos;
    }

    // -------------------------------------------------------------------------
    // TODO A — GET /api/v1/todos
    // -------------------------------------------------------------------------
    // Return all todos. Support optional filtering via ?status=OPEN|IN_PROGRESS|DONE
    // Response: 200 OK with a list of TodoDto (empty list [] if none found)
    // -------------------------------------------------------------------------
    private static async Task<IResult> GetAllTodos(
        [FromQuery] string? status,
        TodoService todoService,
        CancellationToken cancellationToken,
        [FromQuery] int page = 0,
        [FromQuery] int size = 20)
    {
        var todos = await todoService.FindAllAsync(status, page, size, cancellationToken);
        return TypedResults.Ok(todos);
    }

    // -------------------------------------------------------------------------
    // TODO B — GET /api/v1/todos/{id}
    // -------------------------------------------------------------------------
    // Return a single todo by its id.
    // Response: 200 OK with the TodoDto, or 404 if not found (handled globally)
    // -------------------------------------------------------------------------
    private static async Task<IResult> GetTodoById(long id, TodoService todoService, CancellationToken cancellationToken)
    {
        var todo = await todoService.FindByIdAsync(id, cancellationToken);
        return TypedResults.Ok(todo);
    }

    // -------------------------------------------------------------------------
    // TODO C — POST /api/v1/todos
    // -------------------------------------------------------------------------
    // Create a new todo from the request body.
    // Response: 201 Created with the new TodoDto AND a Location header
    //           pointing to the new resource: /api/v1/todos/{id}
    // -------------------------------------------------------------------------
    private static async Task<IResult> CreateTodo(
        [FromBody] CreateTodoRequest request,
        TodoService todoService,
        CancellationToken cancellationToken)
    {
        var todo = await todoService.CreateAsync(request, cancellationToken);
        return TypedResults.Created("/api/v1/todos/{todo.Id}", todo);
    }

    // -------------------------------------------------------------------------
    // TODO D — PUT /api/v1/todos/{id}
    // -------------------------------------------------------------------------
    // Full update of a todo (replace non-null fields from request body).
    // Response: 200 OK with updated TodoDto, or 404 if not found
    // -------------------------------------------------------------------------
    private static async Task<IResult> UpdateTodo(
        long id,
        [FromBody] UpdateTodoRequest request,
        TodoService todoService,
        CancellationToken cancellationToken)
    {
        var todo = await todoService.UpdateAsync(id, request, cancellationToken);
        return TypedResults.Ok(todo);
    }

    // -------------------------------------------------------------------------
    // TODO E — PATCH /api/v1/todos/{id}/status
    // -------------------------------------------------------------------------
    // Update the status of a todo only.
    // Example: PATCH /api/v1/todos/1/status?status=IN_PROGRESS
    // Response: 200 OK with updated TodoDto, or 404 if not found
    // -------------------------------------------------------------------------
    private static async Task<IResult> UpdateStatus(
        long id,
        [FromQuery] TodoStatus status,
        TodoService todoService,
        CancellationToken cancellationToken)
    {
        var todo = await todoService.UpdateStatusAsync(id, status, cancellationToken);
        return TypedResults.Ok(todo);
    }

    // -------------------------------------------------------------------------
    // TODO F — DELETE /api/v1/todos/{id}
    // -------------------------------------------------------------------------
    // Delete a todo by id.
    // Response: 204 No Content (no body), or 404 if not found
    // -------------------------------------------------------------------------
    private static async Task<IResult> DeleteTodo(long id, TodoService todoService, CancellationToken cancellationToken)
    {
        await todoService.DeleteAsync(id, cancellationToken);
        return TypedResults.NoContent();
    }
}
