using System;
using Blogers.Domain.Entities;
using Blogers.Tests.TestHelpers;
using FluentAssertions;

namespace Blogers.Tests.Domain;

public class BlogerTests
{

    [Fact]
    public void Constructor_ValidData_CreatesBloger()
    {
        // Arrange
        var person = TestDataFactory.CreatePerson("Stas");

        // Act
        var bloger = new Bloger(person, "stas_nick", 100);

        // Assert
        bloger.PersonalData.Should().Be(person);
        bloger.Nickname.Should().Be("stas_nick");
        bloger.FollowersNumber.Should().Be(100);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_EmptyNickname_ThrowsArgumentNullException(string? nickname)
    {
        // Arrange
        var person = TestDataFactory.CreatePerson();

        // Act
        Action act = () => new Bloger(person, nickname!, 0);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_NegativeFollowers_ThrowsArgumentOutOfRangeException(int followers)
    {
        // Arrange
        var person = TestDataFactory.CreatePerson();

        // Act
        Action act = () => new Bloger(person, "nick", followers);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_NullFollowers_DefaultsToZero()
    {
        // Arrange
        var person = TestDataFactory.CreatePerson();

        // Act
        var bloger = new Bloger(person, "nick", null);

        // Assert
        bloger.FollowersNumber.Should().Be(0);
    }

    // ---------- Мутаторы ----------

    [Fact]
    public void UpdateNickname_ValidValue_UpdatesNickname()
    {
        // Arrange
        var bloger = TestDataFactory.CreateBloger();

        // Act
        bloger.UpdateNickname("new_nick");

        // Assert
        bloger.Nickname.Should().Be("new_nick");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void UpdateNickname_Empty_ThrowsArgumentNullException(string? nickname)
    {
        // Arrange
        var bloger = TestDataFactory.CreateBloger();

        // Act
        Action act = () => bloger.UpdateNickname(nickname!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void UpdateFollowersNumber_ValidValue_UpdatesFollowers()
    {
        // Arrange
        var bloger = TestDataFactory.CreateBloger();

        // Act
        bloger.UpdateFollowersNumber(500);

        // Assert
        bloger.FollowersNumber.Should().Be(500);
    }

    [Fact]
    public void UpdateFollowersNumber_Null_DefaultsToZero()
    {
        // Arrange
        var bloger = TestDataFactory.CreateBloger();

        // Act
        bloger.UpdateFollowersNumber(null);

        // Assert
        bloger.FollowersNumber.Should().Be(0);
    }

    [Fact]
    public void UpdateFollowersNumber_Negative_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var bloger = TestDataFactory.CreateBloger();

        // Act
        Action act = () => bloger.UpdateFollowersNumber(-5);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ChangeFollowersNumber_Negative_IgnoresChange()
    {
        // Arrange
        var bloger = TestDataFactory.CreateBloger(followers: 100);

        // Act
        bloger.ChangeFollowersNumber(-10);

        // Assert
        bloger.FollowersNumber.Should().Be(100);
    }

    [Fact]
    public void UpdatePersonalData_UpdatesPerson()
    {
        // Arrange
        var bloger = TestDataFactory.CreateBloger();
        var newPerson = new Person("New Name", new DateTime(1995, 5, 5));

        // Act
        bloger.UpdatePersonalData(newPerson);

        // Assert
        bloger.PersonalData.Should().Be(newPerson);
    }

    [Fact]
    public void Stories_ReturnsEmptyReadOnlyList()
    {
        // Arrange & Act
        var bloger = TestDataFactory.CreateBloger();

        // Assert
        bloger.Stories.Should().BeEmpty();
    }

    [Fact]
    public void ChangeFollowersNumber_ValidValue_UpdatesFollowers()
    {
        // Arrange
        var bloger = TestDataFactory.CreateBloger(followers: 100);

        // Act
        bloger.ChangeFollowersNumber(500);

        // Assert
        bloger.FollowersNumber.Should().Be(500);
    }
}