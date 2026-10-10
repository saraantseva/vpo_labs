using System;
using System.Linq;
using System.Threading.Tasks;
using Blogers.Domain.Entities;
using Blogers.Persistense.Data;
using Blogers.Persistense.Repository;
using Blogers.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Blogers.Tests.Integration;

/// <summary>
/// Интеграционные тесты репозиториев на реальном SQLite in-memory.
/// Каждый тест использует свою БД — тесты независимы.
/// </summary>
public class RepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly EfRepository<Bloger> _blogerRepo;
    private readonly EfRepository<Story> _storyRepo;

    public RepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _blogerRepo = new EfRepository<Bloger>(_context);
        _storyRepo = new EfRepository<Story>(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    // ---------- BlogerRepository ----------

    [Fact]
    public async Task BlogerRepository_AddThenGetById_ReturnsEntity()
    {
        // Arrange
        var bloger = TestDataFactory.CreateBloger("stas", 100);

        // Act
        await _blogerRepo.AddAsync(bloger);
        await _context.SaveChangesAsync();

        var fromDb = await _blogerRepo.GetByIdAsync(bloger.Id);

        // Assert
        fromDb.Should().NotBeNull();
        fromDb!.Nickname.Should().Be("stas");
        fromDb.FollowersNumber.Should().Be(100);
    }

    [Fact]
    public async Task BlogerRepository_AddThenListAll_ReturnsAll()
    {
        // Arrange
        await _blogerRepo.AddAsync(TestDataFactory.CreateBloger("a"));
        await _blogerRepo.AddAsync(TestDataFactory.CreateBloger("b"));
        await _context.SaveChangesAsync();

        // Act
        var list = await _blogerRepo.ListAllAsync();

        // Assert
        list.Should().HaveCount(2);
    }

    [Fact]
    public async Task BlogerRepository_DuplicateNickname_ThrowsDbUpdateException()
    {
        // Arrange
        await _blogerRepo.AddAsync(TestDataFactory.CreateBloger("same_nick"));
        await _context.SaveChangesAsync();

        // Act
        await _blogerRepo.AddAsync(TestDataFactory.CreateBloger("same_nick"));
        Func<Task> act = async () => await _context.SaveChangesAsync();

        // Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task BlogerRepository_Delete_RemovesEntity()
    {
        // Arrange
        var bloger = TestDataFactory.CreateBloger();
        await _blogerRepo.AddAsync(bloger);
        await _context.SaveChangesAsync();

        // Act
        await _blogerRepo.DeleteAsync(bloger);
        await _context.SaveChangesAsync();

        var fromDb = await _blogerRepo.GetByIdAsync(bloger.Id);

        // Assert
        fromDb.Should().BeNull();
    }

    [Fact]
    public async Task BlogerRepository_Update_ChangesPersisted()
    {
        // Arrange
        var bloger = TestDataFactory.CreateBloger(nickname: "old", followers: 10);
        await _blogerRepo.AddAsync(bloger);
        await _context.SaveChangesAsync();

        // Act
        bloger.UpdateNickname("new");
        bloger.UpdateFollowersNumber(500);
        await _blogerRepo.UpdateAsync(bloger);
        await _context.SaveChangesAsync();

        // Assert
        _context.ChangeTracker.Clear();
        var fromDb = await _blogerRepo.GetByIdAsync(bloger.Id);
        fromDb!.Nickname.Should().Be("new");
        fromDb.FollowersNumber.Should().Be(500);
    }

    [Fact]
    public async Task BlogerRepository_OwnedPerson_IsPersisted()
    {
        // Arrange
        var bloger = TestDataFactory.CreateBloger();
        await _blogerRepo.AddAsync(bloger);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var fromDb = await _blogerRepo.GetByIdAsync(bloger.Id);

        // Assert
        fromDb!.PersonalData.Should().NotBeNull();
        fromDb.PersonalData.Name.Should().Be("Test Name");
    }

    // ---------- StoryRepository ----------

    [Fact]
    public async Task StoryRepository_AddThenGetById_ReturnsEntity()
    {
        // Arrange
        var story = TestDataFactory.CreateStory("desc", 5, 10, null);

        // Act
        await _storyRepo.AddAsync(story);
        await _context.SaveChangesAsync();

        var fromDb = await _storyRepo.GetByIdAsync(story.Id);

        // Assert
        fromDb.Should().NotBeNull();
        fromDb!.Description.Should().Be("desc");
        fromDb.LikesCount.Should().Be(5);
        fromDb.Status.Should().Be(StoryStatus.Published);
    }

    [Fact]
    public async Task StoryRepository_ListAsync_FiltersByPredicate()
    {
        // Arrange
        var bloger = TestDataFactory.CreateBloger("owner", 0);
        await _blogerRepo.AddAsync(bloger);
        await _context.SaveChangesAsync();

        await _storyRepo.AddAsync(new Story("a", 100, 0, bloger.Id));
        await _storyRepo.AddAsync(new Story("b", 5, 0, bloger.Id));
        await _context.SaveChangesAsync();

        // Act
        var popular = await _storyRepo.ListAsync(s => s.LikesCount >= 50);

        // Assert
        popular.Should().HaveCount(1);
        popular[0].Description.Should().Be("a");
    }

    [Fact]
    public async Task StoryRepository_PersistsStatus()
    {
        // Arrange
        var bloger = TestDataFactory.CreateBloger("status_owner", 0);
        await _blogerRepo.AddAsync(bloger);
        await _context.SaveChangesAsync();

        var story = new Story("draft story", 0, 0, bloger.Id, StoryStatus.Draft);

        // Act
        await _storyRepo.AddAsync(story);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var fromDb = await _storyRepo.GetByIdAsync(story.Id);

        // Assert
        fromDb!.Status.Should().Be(StoryStatus.Draft);
    }

    // ---------- EfUnitOfWork ----------

    [Fact]
    public async Task UnitOfWork_SaveAllAsync_PersistsChanges()
    {
        // Arrange
        var uow = new EfUnitOfWork(_context);
        var bloger = TestDataFactory.CreateBloger("uow_test");

        // Act
        await uow.BlogerRepository.AddAsync(bloger);
        await uow.SaveAllAsync();

        // Assert
        var fromDb = await uow.BlogerRepository.GetByIdAsync(bloger.Id);
        fromDb.Should().NotBeNull();
    }

    [Fact]
    public async Task UnitOfWork_Repositories_ShareSameContext()
    {
        // Arrange
        var uow = new EfUnitOfWork(_context);
        var bloger = TestDataFactory.CreateBloger("shared");
        await uow.BlogerRepository.AddAsync(bloger);
        await uow.SaveAllAsync();

        // Act
        var story = new Story("story for bloger", 0, 0, bloger.Id);
        await uow.StoryRepository.AddAsync(story);
        await uow.SaveAllAsync();

        // Assert
        var fromDb = await uow.StoryRepository.GetByIdAsync(story.Id);
        fromDb!.BlogerId.Should().Be(bloger.Id);
    }
}