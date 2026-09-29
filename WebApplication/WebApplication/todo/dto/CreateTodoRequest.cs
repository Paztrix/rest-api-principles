using System.ComponentModel.DataAnnotations;

namespace WebApplication.todo.dto;

public class CreateTodoRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(255, ErrorMessage = "Title must be at most 255 characters")]
    public string? Title { get; set; }

    [StringLength(1000, ErrorMessage = "Description must be at most 1000 characters")]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }
}
