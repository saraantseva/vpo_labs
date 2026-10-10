namespace Blogers.Domain.Entities;

public class Story : Entity
{
    private Story() { }

    public string Description { get; private set; }
    public int LikesCount { get; private set; }
    public int ViewCount { get; private set; }
    public DateTime CreationDate { get; private set; }
    public StoryStatus Status { get; private set; }

    public int? BlogerId { get; private set; }

    public Story(string description, int? likes, int? views, int? blogerId,
                 StoryStatus status = StoryStatus.Published)
    {
        if (System.String.IsNullOrEmpty(description))
        {
            throw new ArgumentNullException("Description musn't be empty!", nameof(description));
        }

        Description = description;
        LikesCount = likes ?? 0;
        ViewCount = views ?? 0;
        BlogerId = blogerId;
        Status = status;

        if (LikesCount < 0)
        {
            throw new ArgumentOutOfRangeException("Count of likes must be non-negative!", nameof(likes));
        }
        if (ViewCount < 0)
        {
            throw new ArgumentOutOfRangeException("Count of views must be non-negative!", nameof(views));
        }

        CreationDate = DateTime.Now;
    }


    public void Changedescription(string descr)
    {
        if (String.IsNullOrEmpty(descr)) return;
        Description = descr;
    }

    public void AssociateWith(int blogerId)
    {
        if (blogerId < 0) return;
        BlogerId = blogerId;
    }

    public void StopAssociateWith()
    {
        BlogerId = null;
    }

    public void ChangeViewCount(int count)
    {
        if (count <= ViewCount) return;
        ViewCount = count;
    }

    public void ChangeLikesCount(int count)
    {
        if (count < 0) return;
        LikesCount = count;
    }

    // ---------- Переходы состояний ----------

    /// <summary>
    /// Опубликовать историю. Разрешено только из состояния Draft.
    /// </summary>
    public void Publish()
    {
        if (Status == StoryStatus.Published)
        {
            throw new InvalidOperationException("Story is already published.");
        }
        if (Status == StoryStatus.Archived)
        {
            throw new InvalidOperationException("Cannot publish an archived story.");
        }

        Status = StoryStatus.Published;
    }

    /// <summary>
    /// Снять историю с публикации. Разрешено только из состояния Published.
    /// </summary>
    public void Archive()
    {
        if (Status == StoryStatus.Draft)
        {
            throw new InvalidOperationException("Cannot archive a draft story.");
        }
        if (Status == StoryStatus.Archived)
        {
            throw new InvalidOperationException("Story is already archived.");
        }

        Status = StoryStatus.Archived;
    }

    /// <summary>
    /// Вернуть историю из архива в публикацию.
    /// </summary>
    public void Unarchive()
    {
        if (Status != StoryStatus.Archived)
        {
            throw new InvalidOperationException("Only archived story can be unarchived.");
        }

        Status = StoryStatus.Published;
    }
}