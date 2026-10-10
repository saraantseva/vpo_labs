using System;
using Blogers.Domain.Entities;
using Blogers.Tests.TestHelpers;
using FluentAssertions;
using Xunit;

namespace Blogers.Tests.Domain;

public class StoryTests
{
    // ---------- Конструктор ----------

    [Fact]
    public void Constructor_ValidData_CreatesStory()
    {
        // Arrange & Act
        var story = new Story("desc", 5, 10, 1);

        // Assert
        story.Description.Should().Be("desc");
        story.LikesCount.Should().Be(5);
        story.ViewCount.Should().Be(10);
        story.BlogerId.Should().Be(1);
        story.Status.Should().Be(StoryStatus.Published);
    }

    [Fact]
    public void Constructor_DefaultStatus_IsPublished()
    {
        // Arrange & Act
        var story = new Story("desc", 0, 0, 1);

        // Assert
        story.Status.Should().Be(StoryStatus.Published);
    }

    [Fact]
    public void Constructor_ExplicitDraftStatus_CreatesDraft()
    {
        // Arrange & Act
        var story = new Story("desc", 0, 0, 1, StoryStatus.Draft);

        // Assert
        story.Status.Should().Be(StoryStatus.Draft);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_EmptyDescription_ThrowsArgumentNullException(string? desc)
    {
        // Act
        Action act = () => new Story(desc!, 0, 0, 1);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(-5, -5)]
    public void Constructor_NegativeLikesOrViews_ThrowsArgumentOutOfRangeException(int likes, int views)
    {
        // Act
        Action act = () => new Story("desc", likes, views, 1);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_NullLikesAndViews_DefaultsToZero()
    {
        // Act
        var story = new Story("desc", null, null, 1);

        // Assert
        story.LikesCount.Should().Be(0);
        story.ViewCount.Should().Be(0);
    }

    // ---------- Мутаторы ----------

    [Fact]
    public void Changedescription_ValidValue_UpdatesDescription()
    {
        var story = TestDataFactory.CreateStory();

        story.Changedescription("new desc");

        story.Description.Should().Be("new desc");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Changedescription_Empty_IgnoresChange(string? desc)
    {
        var story = TestDataFactory.CreateStory(description: "original");

        story.Changedescription(desc!);

        story.Description.Should().Be("original");
    }

    [Fact]
    public void ChangeLikesCount_ValidValue_Updates()
    {
        var story = TestDataFactory.CreateStory(likes: 5);

        story.ChangeLikesCount(50);

        story.LikesCount.Should().Be(50);
    }

    [Fact]
    public void ChangeLikesCount_Negative_Ignores()
    {
        var story = TestDataFactory.CreateStory(likes: 5);

        story.ChangeLikesCount(-1);

        story.LikesCount.Should().Be(5);
    }

    [Fact]
    public void ChangeViewCount_HigherValue_Updates()
    {
        var story = TestDataFactory.CreateStory(views: 10);

        story.ChangeViewCount(100);

        story.ViewCount.Should().Be(100);
    }

    [Fact]
    public void ChangeViewCount_LowerValue_Ignores()
    {
        var story = TestDataFactory.CreateStory(views: 10);

        story.ChangeViewCount(5);

        story.ViewCount.Should().Be(10);
    }

    [Fact]
    public void AssociateWith_ValidBlogerId_Sets()
    {
        var story = TestDataFactory.CreateStory(blogerId: 1);

        story.AssociateWith(2);

        story.BlogerId.Should().Be(2);
    }

    [Fact]
    public void AssociateWith_NegativeId_Ignores()
    {
        var story = TestDataFactory.CreateStory(blogerId: 1);

        story.AssociateWith(-1);

        story.BlogerId.Should().Be(1);
    }

    [Fact]
    public void StopAssociateWith_SetsBlogerIdToNull()
    {
        var story = TestDataFactory.CreateStory(blogerId: 1);

        story.StopAssociateWith();

        story.BlogerId.Should().BeNull();
    }
}