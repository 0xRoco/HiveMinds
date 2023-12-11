using HiveMinds.API.Controllers;
using HiveMinds.API.Interfaces;
using HiveMinds.DTO;
using HiveMinds.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HiveMinds.Tests.API;

public class RepliesControllerTests
{
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly Mock<IThoughtService> _mockThoughtService;
    private readonly RepliesController _controller;
    
    public RepliesControllerTests()
    {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _mockThoughtService = new Mock<IThoughtService>();
        _controller = new RepliesController(_mockThoughtService.Object, _mockAccountRepository.Object);
    }
    
    
    [Fact]
    public async Task GetRepliesByThoughtId_ReturnsReplies()
    {
        // Arrange
        const int thoughtId = 2;
        var mockReplies = new List<ReplyDto>
        {
            new ReplyDto {ThoughtId = 1, Content = "Reply 1"},
            new ReplyDto {ThoughtId = 2, Content = "Reply 2"},
            new ReplyDto {ThoughtId = 3, Content = "Reply 3"}
        };
        _mockThoughtService.Setup(service => service.GetRepliesByThoughtId(thoughtId))
            .ReturnsAsync(mockReplies);

        // Act
        var result = await _controller.GetRepliesByThoughtId(thoughtId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedReplies = Assert.IsType<List<ReplyDto>>(okResult.Value);
        Assert.Equal(mockReplies.Count, returnedReplies.Count);
    }
    
    [Fact]
    public async Task GetRepliesForUser_ReturnsReplies()
    {
        // Arrange
        const string username = "testuser";
        var mockReplies = new List<ReplyDto>
        {
            new ReplyDto { User = new UserDto {Username = username}, ThoughtId = 1, Content = "Reply 1"},
            new ReplyDto { User = new UserDto {Username = username}, ThoughtId = 2, Content = "Reply 2"},
            new ReplyDto { User = new UserDto {Username = username}, ThoughtId = 3, Content = "Reply 3"},
        };
        _mockThoughtService.Setup(service => service.GetRepliesForUser(username))
            .ReturnsAsync(mockReplies);

        // Act
        var result = await _controller.GetRepliesForUser(username);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedReplies = Assert.IsType<List<ReplyDto>>(okResult.Value);
        Assert.Equal(mockReplies.Count, returnedReplies.Count);
    }
    
    [Fact]
    public async Task GetReplyById_ReturnsReply()
    {
        // Arrange
        const int replyId = 1;
        var mockReply = new ReplyDto { Id = replyId, Content = "Reply 1"};
        _mockThoughtService.Setup(service => service.GetReplyById(replyId))
            .ReturnsAsync(mockReply);

        // Act
        var result = await _controller.GetReplyById(replyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedReply = Assert.IsType<ReplyDto>(okResult.Value);
        Assert.Equal(mockReply.Id, returnedReply.Id);
    }
    
    [Fact]
    public async Task ReplyToThought_CreatesReply()
    {
        // Arrange
        const int thoughtId = 1;
        const string username = "testuser";
        const string body = "Reply content";
        var account = new Account() { Id = 1, Username = username };
        _mockAccountRepository.Setup(repo => repo.GetByUsername(username))
            .ReturnsAsync(account);
        _mockThoughtService.Setup(service => service.ReplyToThought(thoughtId, username, body))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.ReplyToThought(thoughtId, username, body);

        // Assert
        Assert.IsType<OkResult>(result);
    }
    
    [Fact]
    public async Task DeleteReply_DeletesReply()
    {
        // Arrange
        const int replyId = 1;
        _mockThoughtService.Setup(service => service.DeleteReply(replyId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteReply(replyId);

        // Assert
        Assert.IsType<OkResult>(result);
    }
}