using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebApplication.todo.dto;
using WebApplication.todo.exceptions;
using WebApplication.todo.models;
using WebApplication.todo.repositories;
using WebApplication.todo.services;

[assembly: DoNotParallelize]

namespace WebApplication.Tests;

/// <summary>
/// Unit tests for TodoService.
/// These run without an ASP.NET Core host — fast and isolated, using Moq to stub the repository.
/// Run with: dotnet test
/// </summary>
[TestClass]
public class TodoServiceTests
{
    private Mock<ITodoRepository> _todoRepository = null!;
    private TodoService _todoService = null!;
    private TodoModel _sampleTodo = null!;

    [TestInitialize]
    public void SetUp()
    {
        _todoRepository = new Mock<ITodoRepository>();
        _todoService = new TodoService(_todoRepository.Object);
        _sampleTodo = new TodoModel
        {
            Id = 1,
            Title = "Buy milk",
            Status = TodoStatus.OPEN
        };
    }

    // -------------------------------------------------------------------------
    // BONUS TEST 1 — FindAllAsync returns all todos when no status filter
    // -------------------------------------------------------------------------
    [TestMethod]
    public async Task FindAllAsync_NoFilter_ReturnsAllTodos()
    {
        _todoRepository
            .Setup(repository => repository.FindAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([_sampleTodo]);

        var result = await _todoService.FindAllAsync(null);

        Assert.HasCount(1, result);
        Assert.AreEqual("Buy milk", result[0].Title);
        _todoRepository.Verify(repository => repository.FindAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // -------------------------------------------------------------------------
    // BONUS TEST 2 — FindAllAsync with status filter calls FindByStatusAsync
    // -------------------------------------------------------------------------
    [TestMethod]
    public async Task FindAllAsync_WithStatusFilter_ReturnsFilteredTodos()
    {
        _todoRepository
            .Setup(repository => repository.FindByStatusAsync(TodoStatus.OPEN, It.IsAny<CancellationToken>()))
            .ReturnsAsync([_sampleTodo]);

        var result = await _todoService.FindAllAsync("open");

        Assert.HasCount(1, result);
        _todoRepository.Verify(
            repository => repository.FindByStatusAsync(TodoStatus.OPEN, It.IsAny<CancellationToken>()),
            Times.Once);
        _todoRepository.Verify(
            repository => repository.FindAllAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // -------------------------------------------------------------------------
    // BONUS TEST 3 — FindByIdAsync throws when todo does not exist
    // -------------------------------------------------------------------------
    [TestMethod]
    public async Task FindByIdAsync_NotFound_ThrowsResourceNotFoundException()
    {
        _todoRepository
            .Setup(repository => repository.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoModel?)null);

        var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => _todoService.FindByIdAsync(99));

        StringAssert.Contains(exception.Message, "99");
    }

    // -------------------------------------------------------------------------
    // BONUS TEST 4 — CreateAsync saves and returns the new todo
    // -------------------------------------------------------------------------
    [TestMethod]
    public async Task CreateAsync_ValidRequest_ReturnsSavedDto()
    {
        var request = new CreateTodoRequest { Title = "New task" };
        var saved = new TodoModel { Id = 42, Title = "New task", Status = TodoStatus.OPEN };
        _todoRepository
            .Setup(repository => repository.SaveAsync(It.IsAny<TodoModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(saved);

        var result = await _todoService.CreateAsync(request);

        Assert.AreEqual(42, result.Id);
        Assert.AreEqual("New task", result.Title);
        Assert.AreEqual("OPEN", result.Status);
    }
}
