using System;
using Blogers.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Blogers.Tests.Domain;

/// <summary>
/// Тесты переходов состояний Story (Draft → Published → Archived).
/// Соответствуют требованиям FR-17…FR-21.
/// </summary>
public class StoryLifecycleTests
{
    [Fact]
    public void Publish_FromDraft_TransitionsToPublished()
    {
        // Arrange
        var story = new Story("desc", 0, 0, 1, StoryStatus.Draft);

        // Act
        story.Publish();

        // Assert
        story.Status.Should().Be(StoryStatus.Published);
    }

    [Fact]
    public void Publish_FromPublished_ThrowsInvalidOperationException()
    {
        // Arrange
        var story = new Story("desc", 0, 0, 1, StoryStatus.Published);

        // Act
        Action act = () => story.Publish();

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Publish_FromArchived_ThrowsInvalidOperationException()
    {
        // Arrange
        var story = new Story("desc", 0, 0, 1, StoryStatus.Archived);

        // Act
        Action act = () => story.Publish();

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Archive_FromPublished_TransitionsToArchived()
    {
        // Arrange
        var story = new Story("desc", 0, 0, 1, StoryStatus.Published);

        // Act
        story.Archive();

        // Assert
        story.Status.Should().Be(StoryStatus.Archived);
    }

    [Fact]
    public void Archive_FromDraft_ThrowsInvalidOperationException()
    {
        // Arrange
        var story = new Story("desc", 0, 0, 1, StoryStatus.Draft);

        // Act
        Action act = () => story.Archive();

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Archive_FromArchived_ThrowsInvalidOperationException()
    {
        // Arrange
        var story = new Story("desc", 0, 0, 1, StoryStatus.Archived);

        // Act
        Action act = () => story.Archive();

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Unarchive_FromArchived_TransitionsToPublished()
    {
        // Arrange
        var story = new Story("desc", 0, 0, 1, StoryStatus.Archived);

        // Act
        story.Unarchive();

        // Assert
        story.Status.Should().Be(StoryStatus.Published);
    }

    [Theory]
    [InlineData(StoryStatus.Draft)]
    [InlineData(StoryStatus.Published)]
    public void Unarchive_FromNonArchived_ThrowsInvalidOperationException(StoryStatus status)
    {
        // Arrange
        var story = new Story("desc", 0, 0, 1, status);

        // Act
        Action act = () => story.Unarchive();

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
}