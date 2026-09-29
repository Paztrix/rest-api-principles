using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using WebApplication.todo.data;
using WebApplication.todo.endpoints;
using WebApplication.todo.exceptions;
using WebApplication.todo.repositories;
using WebApplication.todo.services;

var builder = global::Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("Todos") ??
    $"Host={configuration["DB_HOST"] ?? "localhost"};" +
    $"Port={configuration["DB_PORT"] ?? "5432"};" +
    $"Database={configuration["DB_NAME"] ?? "todos"};" +
    $"Username={configuration["DB_USER"] ?? "todos"};" +
    $"Password={configuration["DB_PASSWORD"] ?? "todos"}";

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddValidation();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<TodoService>();
builder.Services.AddExceptionHandler<GlobalExceptionHandling>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

// Equivalent to HelloController's GET / endpoint.
app.MapGet("/", () => "Hello, API is running");
app.MapTodoEndpoints();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();
