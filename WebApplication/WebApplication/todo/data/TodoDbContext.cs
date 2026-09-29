using Microsoft.EntityFrameworkCore;
using WebApplication.todo.models;

namespace WebApplication.todo.data;

public sealed class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<TodoModel> Todos => Set<TodoModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var todo = modelBuilder.Entity<TodoModel>();

        todo.ToTable("todos");
        todo.HasKey(item => item.Id);
        todo.Property(item => item.Id).ValueGeneratedOnAdd();
        todo.Property(item => item.Title).IsRequired().HasMaxLength(255);
        todo.Property(item => item.Status).HasConversion<string>().IsRequired();
        todo.Property(item => item.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .ValueGeneratedNever();
        todo.Property(item => item.DueDate).HasColumnType("timestamp without time zone");
    }
}
