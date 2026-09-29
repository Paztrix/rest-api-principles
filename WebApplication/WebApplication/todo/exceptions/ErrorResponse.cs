namespace WebApplication.todo.exceptions;

public class ErrorResponse
{
    public ErrorResponse(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
