using HiveMinds.API.Controllers;
using HiveMinds.API.Services.Interfaces;
using HiveMinds.DTO;
using HiveMinds.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HiveMinds.Tests.API;

public class ThoughtsControllerTests
{
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly Mock<IThoughtService> _mockThoughtService;
    private readonly ThoughtsController _controller;
    
    public ThoughtsControllerTests()
    {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _mockThoughtService = new Mock<IThoughtService>();
        _controller = new ThoughtsController(_mockAccountRepository.Object, _mockThoughtService.Object);
    }
    
    [Fact]
    public async Task GetThoughts_ReturnsAllThoughts()
    {
        // Arrange
        var mockThoughts = new List<ThoughtDto>
        {
            new ThoughtDto { Id = 1, Content = "Thought 1" },
            new ThoughtDto { Id = 2, Content = "Thought 2" },
            new ThoughtDto { Id = 3, Content = "Thought 3" }
        };
        
        _mockThoughtService.Setup(service => service.GetThoughts())
            .ReturnsAsync(mockThoughts);

        // Act
        var result = await _controller.GetThoughts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedThoughts = Assert.IsType<List<ThoughtDto>>(okResult.Value);
        Assert.Equal(mockThoughts.Count, returnedThoughts.Count);
    }
    
    [Fact]
    public async Task GetThought_ReturnsThoughtById()
    {
        // Arrange
        const int thoughtId = 2;
        var mockThoughts = new List<ThoughtDto>
        {
            new ThoughtDto { Id = 1, Content = "Thought 1" },
            new ThoughtDto { Id = thoughtId, Content = "Thought 2" },
            new ThoughtDto { Id = 3, Content = "Thought 3" }
        };
        
        _mockThoughtService.Setup(service => service.GetThoughtById(thoughtId))
            .ReturnsAsync(mockThoughts[thoughtId - 1]);

        // Act
        var result = await _controller.GetThought(thoughtId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedThought = Assert.IsType<ThoughtDto>(okResult.Value);
        Assert.Equal(thoughtId, returnedThought.Id);
    }
    
    [Fact]
    public async Task GetThoughtsByUsername_ReturnsThoughtsForUser()
    {
        // Arrange
        var username = "testuser";
        var mockThoughts = new List<ThoughtDto>
        {
            new ThoughtDto
            {
                Id = 1,
                User = new UserDto { Username = username },
                Content = "Thought 1",
                Replies = new List<ReplyDto>(),
                Likes = new List<LikeDto>(),
                CreatedAt = default
            },
            new ThoughtDto
            {
                Id = 2,
                User = new UserDto { Username = username },
                Content = "Thought 2",
                Replies = new List<ReplyDto>(),
                Likes = new List<LikeDto>(),
                CreatedAt = default
            },
        };
        _mockThoughtService.Setup(service => service.GetThoughtsByUsername(username))
            .ReturnsAsync(mockThoughts);

        // Act
        var result = await _controller.GetThoughtsByUsername(username);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedThoughts = Assert.IsType<List<ThoughtDto>>(okResult.Value);
        Assert.Equal(mockThoughts.Count, returnedThoughts.Count);
    }
    
    [Fact]
    public async Task CreateThought_CreatesNewThought()
    {
        // Arrange
        const string username = "testuser";
        const string body = "Test thought content";
        _mockAccountRepository.Setup(repo => repo.GetByUsername(username))
            .ReturnsAsync(new Account {Username = username});
        _mockThoughtService.Setup(service => service.CreateThought(username, body))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CreateThought(username, body);

        // Assert
        Assert.IsType<OkResult>(result);
    }
    
    [Fact]
    public async Task DeleteThought_DeletesThoughtById()
    {
        // Arrange
        const int thoughtId = 1;
        _mockThoughtService.Setup(service => service.DeleteThought(thoughtId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteThought(thoughtId);

        // Assert
        Assert.IsType<OkResult>(result);
    }
    
}