using HiveMinds.API.Controllers;
using HiveMinds.API.Services.Interfaces;
using HiveMinds.DTO;
using HiveMinds.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HiveMinds.Tests.API;

public class LikesControllerTests
{
    private readonly Mock<IThoughtService> _mockThoughtService;
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly LikesController _controller;
    
    public LikesControllerTests()
    {
        _mockThoughtService = new Mock<IThoughtService>();
        _mockAccountRepository = new Mock<IAccountRepository>();
        _controller = new LikesController(_mockThoughtService.Object, _mockAccountRepository.Object);
    }

    [Fact]
    public async Task GetLikesByThoughtId_ReturnsLikes()
    {
        // Arrange
        const int thoughtId = 1;
        var mockLikes = new List<LikeDto>
        {
            new LikeDto { Id = 1, ThoughtId = thoughtId },
            new LikeDto { Id = 2, ThoughtId = thoughtId },
            new LikeDto { Id = 3, ThoughtId = thoughtId }
        };
        _mockThoughtService.Setup(service => service.GetLikesByThoughtId(thoughtId))
            .ReturnsAsync(mockLikes);

        // Act
        var result = await _controller.GetLikesByThoughtId(thoughtId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedLikes = Assert.IsType<List<LikeDto>>(okResult.Value);
        Assert.Equal(mockLikes.Count, returnedLikes.Count);
    }
    
    [Fact]
    public async Task GetLikesForUser_ReturnsLikes()
    {
        // Arrange
        const string username = "testuser";
        var mockAccount = new Account() { Id = 1 };
        var mockLikes = new List<LikeDto>
        {
            new LikeDto { Id = 1, ThoughtId = 2},
            new LikeDto { Id = 2, ThoughtId = 3},
        };
        _mockAccountRepository.Setup(repo => repo.GetByUsername(username))
            .ReturnsAsync(mockAccount);
        _mockThoughtService.Setup(service => service.GetLikesForUser(mockAccount.Id))
            .ReturnsAsync(mockLikes);

        // Act
        var result = await _controller.GetLikesForUser(username);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedLikes = Assert.IsType<List<LikeDto>>(okResult.Value);
        Assert.Equal(mockLikes.Count, returnedLikes.Count);
    }
    
    [Fact]
    public async Task GetLikeById_ReturnsLike()
    {
        // Arrange
        const int likeId = 1;
        var mockLike = new LikeDto { Id = likeId, ThoughtId = 2};
        _mockThoughtService.Setup(service => service.GetLikeById(likeId))
            .ReturnsAsync(mockLike);

        // Act
        var result = await _controller.GetLikeById(likeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedLike = Assert.IsType<LikeDto>(okResult.Value);
        Assert.Equal(mockLike.Id, returnedLike.Id);
    }
    
    [Fact]
    public async Task LikeThought_CreatesLike()
    {
        // Arrange
        const int thoughtId = 1;
        const string username = "testuser";
        var mockAccount = new Account() { Id = 1, Username = username};
        _mockAccountRepository.Setup(repo => repo.GetByUsername(username))
            .ReturnsAsync(mockAccount);
        _mockThoughtService.Setup(service => service.LikeThought(thoughtId, username))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.LikeThought(thoughtId, username);

        // Assert
        Assert.IsType<OkResult>(result);
    }
    
    [Fact]
    public async Task DeleteLike_RemovesLike()
    {
        // Arrange
        const int thoughtId = 1;
        const string username = "testuser";
        _mockThoughtService.Setup(service => service.UnlikeThought(thoughtId, username))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteLike(thoughtId, username);

        // Assert
        Assert.IsType<OkResult>(result);
    }

}