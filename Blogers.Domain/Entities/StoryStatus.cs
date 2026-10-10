using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.Domain.Entities
{
    /// <summary>
    /// Жизненный цикл истории.
    /// Draft     — черновик (создана, ещё не опубликована)
    /// Published — опубликована
    /// Archived  — снята с публикации (конечное состояние)
    /// </summary>
    public enum StoryStatus
    {
        Draft = 0,
        Published = 1,
        Archived = 2
    }
}