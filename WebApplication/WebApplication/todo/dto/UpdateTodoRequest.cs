using System.ComponentModel.DataAnnotations;
using WebApplication.todo.models;

namespace WebApplication.todo.dto;

public class UpdateTodoRequest
{
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters")]
    public string? Title { get; set; }

    [StringLength(1000, ErrorMessage = "Description must be at most 1000 characters")]
    public string? Description { get; set; }

    public TodoStatus? Status { get; set; }

    public DateTime? DueDate { get; set; }
}
