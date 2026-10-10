using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Blogers.Application.StoryUseCases.Commands;
using Blogers.Application.StoryUseCases.Queries;
using Blogers.Domain.Abstractions;
using Blogers.Domain.Entities;
using Blogers.Tests.TestHelpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Blogers.Tests.Application;

public class StoryHandlersTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IRepository<Story>> _storyRepoMock;

    public StoryHandlersTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _storyRepoMock = new Mock<IRepository<Story>>();
        _uowMock.Setup(u => u.StoryRepository).Returns(_storyRepoMock.Object);
        _uowMock.Setup(u => u.SaveAllAsync()).Returns(Task.CompletedTask);
    }

    // ---------- AddStoryHandler ----------

    [Fact]
    public async Task AddStoryHandler_ValidData_AddsAndSaves()
    {
        // Arrange
        var handler = new AddStoryHandler(_uowMock.Object);
        var request = new AddStoryRequest("desc", 5, 10, 1);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be("desc");
        result.LikesCount.Should().Be(5);
        result.ViewCount.Should().Be(10);
        result.BlogerId.Should().Be(1);
        _storyRepoMock.Verify(r => r.AddAsync(It.IsAny<Story>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveAllAsync(), Times.Once);
    }

    // ---------- DeleteStoryHandler ----------

    [Fact]
    public async Task DeleteStoryHandler_ExistingId_DeletesAndSaves()
    {
        // Arrange
        var story = TestDataFactory.CreateStory();
        _storyRepoMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Story, object>>[]?>()))
            .ReturnsAsync(story);

        var handler = new DeleteStoryHandler(_uowMock.Object);

        // Act
        await handler.Handle(new DeleteStoryCommand(1), CancellationToken.None);

        // Assert
        _storyRepoMock.Verify(r => r.DeleteAsync(story, It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveAllAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteStoryHandler_MissingId_DoesNothing()
    {
        // Arrange
        _storyRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Story, object>>[]?>()))
            .ReturnsAsync((Story?)null);

        var handler = new DeleteStoryHandler(_uowMock.Object);

        // Act
        await handler.Handle(new DeleteStoryCommand(999), CancellationToken.None);

        // Assert
        _storyRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Story>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ---------- EditStoryHandler ----------

    [Fact]
    public async Task EditStoryHandler_ExistingId_UpdatesFields()
    {
        // Arrange
        var story = TestDataFactory.CreateStory(description: "old", likes: 1, views: 1, blogerId: 1);
        _storyRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Story, object>>[]?>()))
            .ReturnsAsync(story);

        var handler = new EditStoryHandler(_uowMock.Object);
        var request = new EditStoryRequest(1, "new", 100, 200, 2);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Description.Should().Be("new");
        result.LikesCount.Should().Be(100);
        result.BlogerId.Should().Be(2);
        _storyRepoMock.Verify(r => r.UpdateAsync(story, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EditStoryHandler_MissingId_ReturnsNull()
    {
        // Arrange
        _storyRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Story, object>>[]?>()))
            .ReturnsAsync((Story?)null);

        var handler = new EditStoryHandler(_uowMock.Object);
        var request = new EditStoryRequest(999, "x", 0, 0, 1);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    // ---------- GetAllStoriesHandler ----------

    [Fact]
    public async Task GetAllStoriesHandler_ReturnsAllStories()
    {
        // Arrange
        var stories = new List<Story>
        {
            TestDataFactory.CreateStory("a"),
            TestDataFactory.CreateStory("b"),
            TestDataFactory.CreateStory("c")
        };
        _storyRepoMock
            .Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(stories);

        var handler = new GetAllStoriesHandler(_uowMock.Object);

        // Act
        var result = await handler.Handle(new GetAllStoriesRequest(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        _storyRepoMock.Verify(r => r.ListAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllStoriesHandler_EmptyRepository_ReturnsEmpty()
    {
        // Arrange
        _storyRepoMock
            .Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Story>());

        var handler = new GetAllStoriesHandler(_uowMock.Object);

        // Act
        var result = await handler.Handle(new GetAllStoriesRequest(), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    // ---------- GetStoriesByBlogerIdHandle ----------

    [Fact]
    public async Task GetStoriesByBlogerIdHandle_ExistingBloger_ReturnsMatchingStories()
    {
        // Arrange
        var blogerId = 1;
        var stories = new List<Story>
        {
            TestDataFactory.CreateStory("a", blogerId: blogerId),
            TestDataFactory.CreateStory("b", blogerId: blogerId)
        };
        _storyRepoMock
            .Setup(r => r.ListAsync(
                It.IsAny<Expression<Func<Story, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Story, object>>[]?>()))
            .ReturnsAsync(stories);

        var handler = new GetStoriesByBlogerIdHandle(_uowMock.Object);

        // Act
        var result = await handler.Handle(
            new GetStoriesByBlogerIdRequest(blogerId),
            CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        _storyRepoMock.Verify(r => r.ListAsync(
            It.IsAny<Expression<Func<Story, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Story, object>>[]?>()), Times.Once);
    }

    [Fact]
    public async Task GetStoriesByBlogerIdHandle_NoStories_ReturnsEmpty()
    {
        // Arrange
        _storyRepoMock
            .Setup(r => r.ListAsync(
                It.IsAny<Expression<Func<Story, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Story, object>>[]?>()))
            .ReturnsAsync(new List<Story>());

        var handler = new GetStoriesByBlogerIdHandle(_uowMock.Object);

        // Act
        var result = await handler.Handle(
            new GetStoriesByBlogerIdRequest(999),
            CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}