namespace WebApplication.todo.exceptions;

public class ResourceNotFoundException(string message) : Exception(message)
{
    public static ResourceNotFoundException ForTodo(long id) =>
        new($"Todo not found with id: {id}");
}
