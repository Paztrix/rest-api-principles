using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.todo.models;

public class TodoModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TodoStatus Status { get; set; } = TodoStatus.OPEN;

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? DueDate { get; set; }
}
