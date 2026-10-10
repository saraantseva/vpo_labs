using System;
using Blogers.Domain.Entities;

namespace Blogers.Tests.TestHelpers;

/// <summary>
/// Фабрика тестовых данных. Единая точка создания сущностей для тестов.
/// </summary>
public static class TestDataFactory
{
    public static Person CreatePerson(string name = "Test Name")
        => new Person(name, new DateTime(2000, 1, 1));

    public static Bloger CreateBloger(string nickname = "test_nick", int followers = 100)
        => new Bloger(CreatePerson(), nickname, followers);

    public static Story CreateStory(string description = "Test story",
                                    int likes = 5,
                                    int views = 10,
                                    int? blogerId = 1,
                                    StoryStatus status = StoryStatus.Published)
        => new Story(description, likes, views, blogerId, status);
}