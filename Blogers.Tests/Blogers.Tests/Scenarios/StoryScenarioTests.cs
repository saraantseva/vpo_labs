using System;
using System.Threading.Tasks;
using Blogers.Domain.Entities;
using Blogers.Persistense.Data;
using Blogers.Persistense.Repository;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Blogers.Tests.Scenarios;

/// <summary>
/// Сценарные тесты: полный жизненный цикл Story.
/// Соответствуют FR-17…FR-21 из спецификации требований.
/// </summary>
public class StoryScenarioTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly EfUnitOfWork _uow;

    public StoryScenarioTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _uow = new EfUnitOfWork(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task Scenario_Draft_Publish_Archive_Unarchive_PersistsAllTransitions()
    {
        // Arrange
        var story = new Story("draft story", 0, 0, null, StoryStatus.Draft);
        await _uow.StoryRepository.AddAsync(story);
        await _uow.SaveAllAsync();

        story.Status.Should().Be(StoryStatus.Draft);

        // Act 1: публикация
        story.Publish();
        await _uow.StoryRepository.UpdateAsync(story);
        await _uow.SaveAllAsync();

        // Assert 1
        _context.ChangeTracker.Clear();
        var afterPublish = await _uow.StoryRepository.GetByIdAsync(story.Id);
        afterPublish!.Status.Should().Be(StoryStatus.Published);

        // Act 2: архивация
        afterPublish.Archive();
        await _uow.StoryRepository.UpdateAsync(afterPublish);
        await _uow.SaveAllAsync();

        // Assert 2
        _context.ChangeTracker.Clear();
        var afterArchive = await _uow.StoryRepository.GetByIdAsync(story.Id);
        afterArchive!.Status.Should().Be(StoryStatus.Archived);

        // Act 3: разархивация
        afterArchive.Unarchive();
        await _uow.StoryRepository.UpdateAsync(afterArchive);
        await _uow.SaveAllAsync();

        // Assert 3
        _context.ChangeTracker.Clear();
        var afterUnarchive = await _uow.StoryRepository.GetByIdAsync(story.Id);
        afterUnarchive!.Status.Should().Be(StoryStatus.Published);
    }

    [Fact]
    public async Task Scenario_ArchiveDraft_ThrowsAndDoesNotChangeState()
    {
        // Arrange
        var story = new Story("draft", 0, 0, null, StoryStatus.Draft);

        // Act
        Action act = () => story.Archive();

        // Assert
        act.Should().Throw<InvalidOperationException>();
        story.Status.Should().Be(StoryStatus.Draft);
    }

    [Fact]
    public async Task Scenario_FullBlogerAndStoryLifecycle()
    {
        // Arrange — создаём блогера и историю
        var bloger = new Bloger(new Person("Stas", new DateTime(2000, 1, 1)), "stas", 100);
        await _uow.BlogerRepository.AddAsync(bloger);
        await _uow.SaveAllAsync();

        var story = new Story("first story", 5, 10, bloger.Id, StoryStatus.Draft);
        await _uow.StoryRepository.AddAsync(story);
        await _uow.SaveAllAsync();

        // Act — публикуем
        story.Publish();
        await _uow.StoryRepository.UpdateAsync(story);
        await _uow.SaveAllAsync();

        // Assert — история сохранена и связана с блогером
        _context.ChangeTracker.Clear();
        var fromDb = await _uow.StoryRepository.GetByIdAsync(story.Id);
        fromDb!.Status.Should().Be(StoryStatus.Published);
        fromDb.BlogerId.Should().Be(bloger.Id);

        // Act — удаляем историю
        await _uow.StoryRepository.DeleteAsync(fromDb);
        await _uow.SaveAllAsync();

        // Assert
        _context.ChangeTracker.Clear();
        var deleted = await _uow.StoryRepository.GetByIdAsync(story.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task Scenario_PersistedDataSurvivesContextRecreation()
    {
        // Arrange
        var bloger = new Bloger(new Person("Name", new DateTime(2000, 1, 1)), "persistent_nick", 50);
        await _uow.BlogerRepository.AddAsync(bloger);
        await _uow.SaveAllAsync();

        // Act — пересоздаём контекст поверх той же in-memory БД
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        await using var newContext = new AppDbContext(options);
        var newUow = new EfUnitOfWork(newContext);
        var fromDb = await newUow.BlogerRepository.GetByIdAsync(bloger.Id);

        // Assert
        fromDb.Should().NotBeNull();
        fromDb!.Nickname.Should().Be("persistent_nick");
    }
}