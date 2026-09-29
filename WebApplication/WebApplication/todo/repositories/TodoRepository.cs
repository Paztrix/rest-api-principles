using Microsoft.EntityFrameworkCore;
using WebApplication.todo.data;
using WebApplication.todo.models;

namespace WebApplication.todo.repositories;

public interface ITodoRepository
{
    Task<List<TodoModel>> FindAllAsync(CancellationToken cancellationToken = default);
    Task<List<TodoModel>> FindByStatusAsync(TodoStatus status, CancellationToken cancellationToken = default);
    Task<TodoModel?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<TodoModel> SaveAsync(TodoModel todo, CancellationToken cancellationToken = default);
    Task DeleteAsync(TodoModel todo, CancellationToken cancellationToken = default);
    Task<int> CountAsync(TodoStatus? status, CancellationToken cancellationToken = default);
    Task<List<TodoModel>> FindPageAsync(TodoStatus? status, int page, int size, CancellationToken cancellationToken = default);
}

public sealed class TodoRepository(TodoDbContext dbContext) : ITodoRepository
{
    public Task<List<TodoModel>> FindAllAsync(CancellationToken cancellationToken = default) =>
        dbContext.Todos.ToListAsync(cancellationToken);

    public Task<int> CountAsync(TodoStatus? status, CancellationToken cancellationToken = default) 
    {
        var query = dbContext.Todos.AsQueryable();

        if(status != null) 
        {
            query = query.Where(todo => todo.Status == status);
        }

        return query.CountAsync(cancellationToken);
    }

    public Task<List<TodoModel>> FindPageAsync(TodoStatus? status, int page, int size, CancellationToken cancellationToken = default) 
    {
        var query = dbContext.Todos.AsQueryable();

        if (status != null) {
            query = query.Where(todo => todo.Status == status);
        }

        return query
            .OrderByDescending(todo => todo.CreatedAt)
            .Skip(page * size)
            .Take(size)
            .ToListAsync(cancellationToken);
    }

    public Task<List<TodoModel>> FindByStatusAsync(TodoStatus status, CancellationToken cancellationToken = default) =>
        dbContext.Todos.Where(todo => todo.Status == status).ToListAsync(cancellationToken);

    public Task<TodoModel?> FindByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Todos.FindAsync([id], cancellationToken).AsTask();

    public async Task<TodoModel> SaveAsync(TodoModel todo, CancellationToken cancellationToken = default)
    {
        if (todo.Id == 0)
        {
            await dbContext.Todos.AddAsync(todo, cancellationToken);
        }
        else
        {
            dbContext.Todos.Update(todo);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return todo;
    }

    public async Task DeleteAsync(TodoModel todo, CancellationToken cancellationToken = default)
    {
        dbContext.Todos.Remove(todo);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
