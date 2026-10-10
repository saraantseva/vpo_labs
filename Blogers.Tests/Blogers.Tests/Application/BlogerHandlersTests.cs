using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Blogers.Application.BlogerUseCases.Commands;
using Blogers.Application.BlogerUseCases.Queries;
using Blogers.Domain.Abstractions;
using Blogers.Domain.Entities;
using Blogers.Tests.TestHelpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Blogers.Tests.Application;

public class BlogerHandlersTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IRepository<Bloger>> _blogerRepoMock;

    public BlogerHandlersTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _blogerRepoMock = new Mock<IRepository<Bloger>>();
        _uowMock.Setup(u => u.BlogerRepository).Returns(_blogerRepoMock.Object);
        _uowMock.Setup(u => u.SaveAllAsync()).Returns(Task.CompletedTask);
    }

    // ---------- AddBlogerHandler ----------

    [Fact]
    public async Task AddBlogerHandler_ValidData_AddsAndSaves()
    {
        // Arrange
        var handler = new AddBlogerHandler(_uowMock.Object);
        var command = new AddBlogerCommand("Stas", new DateTime(2000, 1, 1), "stas_nick", 100);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Nickname.Should().Be("stas_nick");
        result.FollowersNumber.Should().Be(100);
        _blogerRepoMock.Verify(r => r.AddAsync(It.IsAny<Bloger>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveAllAsync(), Times.Once);
    }

    [Fact]
    public async Task AddBlogerHandler_NullFollowers_DefaultsToZero()
    {
        // Arrange
        var handler = new AddBlogerHandler(_uowMock.Object);
        var command = new AddBlogerCommand("Name", DateTime.Now, "nick", null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.FollowersNumber.Should().Be(0);
    }

    // ---------- DeleteBlogerHandler ----------

    [Fact]
    public async Task DeleteBlogerHandler_ExistingId_DeletesAndSaves()
    {
        // Arrange
        var bloger = TestDataFactory.CreateBloger();
        _blogerRepoMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Bloger, object>>[]?>()))
            .ReturnsAsync(bloger);

        var handler = new DeleteBlogerHandler(_uowMock.Object);

        // Act
        await handler.Handle(new DeleteBlogerCommand(1), CancellationToken.None);

        // Assert
        _blogerRepoMock.Verify(r => r.DeleteAsync(bloger, It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveAllAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteBlogerHandler_MissingId_DoesNothing()
    {
        // Arrange
        _blogerRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Bloger, object>>[]?>()))
            .ReturnsAsync((Bloger?)null);

        var handler = new DeleteBlogerHandler(_uowMock.Object);

        // Act
        await handler.Handle(new DeleteBlogerCommand(999), CancellationToken.None);

        // Assert
        _blogerRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Bloger>(), It.IsAny<CancellationToken>()), Times.Never);
        _uowMock.Verify(u => u.SaveAllAsync(), Times.Never);
    }

    // ---------- EditBlogerHandler ----------

    [Fact]
    public async Task EditBlogerHandler_ExistingId_UpdatesFields()
    {
        // Arrange
        var bloger = TestDataFactory.CreateBloger(nickname: "old_nick", followers: 10);
        _blogerRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Bloger, object>>[]?>()))
            .ReturnsAsync(bloger);

        var handler = new EditBlogerHandler(_uowMock.Object);
        var command = new EditBlogerCommand(1, "New Name", new DateTime(1995, 5, 5), "new_nick", 500);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Nickname.Should().Be("new_nick");
        result.FollowersNumber.Should().Be(500);
        _blogerRepoMock.Verify(r => r.UpdateAsync(bloger, It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveAllAsync(), Times.Once);
    }

    [Fact]
    public async Task EditBlogerHandler_MissingId_ReturnsNull()
    {
        // Arrange
        _blogerRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Bloger, object>>[]?>()))
            .ReturnsAsync((Bloger?)null);

        var handler = new EditBlogerHandler(_uowMock.Object);
        var command = new EditBlogerCommand(999, "X", DateTime.Now, "x", 0);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    // ---------- GetAllBlogersHandler ----------

    [Fact]
    public async Task GetAllBlogersHandler_ReturnsAllBlogers()
    {
        // Arrange
        var list = new List<Bloger>
        {
            TestDataFactory.CreateBloger("a"),
            TestDataFactory.CreateBloger("b")
        };
        _blogerRepoMock
            .Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var handler = new GetAllBlogersHandler(_uowMock.Object);

        // Act
        var result = await handler.Handle(new GetAllBlogersRequest(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    // ---------- GetBlogerHandler ----------

    [Fact]
    public async Task GetBlogerHandler_ExistingId_ReturnsBloger()
    {
        // Arrange
        var bloger = TestDataFactory.CreateBloger();
        _blogerRepoMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Bloger, object>>[]?>()))
            .ReturnsAsync(bloger);

        var handler = new GetBlogerHandler(_uowMock.Object);

        // Act
        var result = await handler.Handle(new GetBlogerRequest(1), CancellationToken.None);

        // Assert
        result.Should().Be(bloger);
    }
}