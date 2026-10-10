# Журнал дефектов

Дефекты, выявленные при верификации требований
(см. [`docs/requirements.md`](docs/requirements.md), раздел 6.4).

## Формат записи
| Поле | Описание |
|---|---|
| ID | Уникальный идентификатор `ISSUE-N` |
| Заголовок | Краткое описание проблемы |
| Требование | Ссылка на нарушенное требование |
| Серьёзность | Критическая / Высокая / Средняя / Низкая / Косметика |
| Статус | Открыт / В работе / Закрыт |

## Сводная таблица

| ID | Заголовок | Требование | Серьёзность | Статус |
|---|---|---|---|---|
| ISSUE-1 | `DbInitializer.Initialize` удаляет БД при каждом запуске | NFR-3 | Высокая | Открыт |
| ISSUE-2 | `CreateBlogerPageViewModel.Save` содержит мёртвую проверку `DateOfBirth == null` | FR-2 | Низкая | Открыт |
| ISSUE-3 | `FakeStoryRepository.GetByIdAsync` ищет по `BlogerId`, а не по `Id` | FR-13 | Средняя | Открыт |
| ISSUE-4 | `ChangeStoryPageViewModel.LoadData` вызывается дважды (XAML + code-behind) | FR-12 | Низкая | Открыт |
| ISSUE-5 | Дублирование валидации в `Bloger` (конструктор + Update-методы) | FR-3, FR-4 | Низкая | Открыт |
| ISSUE-6 | Handler-ы возвращают `null` при non-nullable сигнатуре | FR-6, FR-12 | Средняя | Открыт |
| ISSUE-7 | `Picker.SelectedIndexChanged` может сработать до обновления `SelectedItem` | FR-8 | Средняя | Открыт |
| ISSUE-8 | `DbInitializer` в слое Application зависит от DI | NFR-4 | Низкая | Открыт |
| ISSUE-9 | `EfUnitOfWork` подключает `using Abp.Domain.Uow;` | NFR-4 | Низкая | Открыт |
| ISSUE-10 | Singleton-регистрация `AppDbContext` и `IUnitOfWork` | NFR-3, NFR-4 | Средняя | Открыт |
| ISSUE-11 | `MauiProgram` создаёт второй `ServiceProvider` | NFR-4 | Средняя | Открыт |
| ISSUE-12 | `AppDbContext.EnsureCreated()` вызывается в конструкторе | NFR-3 | Низкая | Открыт |
| ISSUE-13 | Опечатка «Добавить иторию» в `BlogersPage.xaml` | — | Косметика | Открыт |
| ISSUE-14 | `LikeToColorValueConverter` не защищён от `null` | FR-22 | Низкая | Открыт |

## Подробное описание

### ISSUE-1: `DbInitializer` удаляет БД при каждом запуске
- **Где:** `Blogers.Application/DbInitializer.cs`, метод `Initialize`.
- **Проблема:** перед созданием seed-данных выполняются `DeleteDataBaseAsync()` и
  `CreateDataBaseAsync()`. Это приводит к полной потере пользовательских данных
  при каждом запуске приложения.
- **Шаги воспроизведения:**
  1. Запустить приложение.
  2. Добавить блогера.
  3. Закрыть и снова запустить приложение.
- **Ожидаемое:** блогер сохранился; seed выполняется только для пустой БД.
- **Фактическое:** блогер исчез, отображаются только seed-данные.
- **Нарушенное требование:** NFR-3.
- **Предлагаемое исправление:** переносить seed в отдельный `IHostedService`,
  вызывать только при `!await context.Blogers.AnyAsync()`.

### ISSUE-2: мёртвая проверка `DateOfBirth == null`
- **Где:** `Blogers.UI/ViewModels/CreateBlogerPageViewModel.cs`, метод `Save`.
- **Проблема:** `DateOfBirth` имеет тип `DateTime` (value type), условие
  `DateOfBirth == null` всегда ложно.
- **Нарушенное требование:** FR-2.
- **Предлагаемое исправление:** удалить проверку; ограничение даты реализовано
  через `MaximumDate` в XAML.

### ISSUE-3: `FakeStoryRepository.GetByIdAsync` ищет по `BlogerId`
- **Где:** `Blogers.Persistense/Repository/FakeStoryRepository.cs`.
- **Проблема:** реализация `_stories.FirstOrDefault(s => s.BlogerId == id)` не
  соответствует контракту `IRepository<T>.GetByIdAsync` (должен искать по `Id`).
- **Нарушенное требование:** FR-13.
- **Предлагаемое исправление:** заменить на `s => s.Id == id`.

### ISSUE-4: двойной вызов `LoadData`
- **Где:** `ChangeStoryPage.xaml` (`EventToCommandBehavior Loaded`) и
  `ChangeStoryPage.xaml.cs` (`OnNavigatedTo`).
- **Проблема:** данные загружаются дважды.
- **Нарушенное требование:** FR-12 (по производительности).
- **Предлагаемое исправление:** убрать один из источников вызова.

### ISSUE-5: дублирование валидации в `Bloger`
- **Где:** `Blogers.Domain/Entities/Bloger.cs`.
- **Проблема:** проверки в конструкторе дублируются в `UpdateNickname` и
  `UpdateFollowersNumber`.
- **Нарушенное требование:** FR-3, FR-4.
- **Предлагаемое исправление:** вынести проверки в приватные методы и
  вызывать их из конструктора и мутаторов.

### ISSUE-6: возврат `null` в non-nullable сигнатуре
- **Где:** `EditBlogerHandler`, `EditStoryHandler`.
- **Проблема:** сигнатура `Task<Bloger>` / `Task<Story>`, фактически может
  вернуться `null`.
- **Нарушенное требование:** FR-6, FR-12.
- **Предлагаемое исправление:** `Task<Bloger?>` / `Task<Story?>` или выброс
  `InvalidOperationException`.

### ISSUE-7: `Picker.SelectedIndexChanged` и `SelectedItem`
- **Где:** `BlogersPage.xaml`.
- **Проблема:** `UpdateStoriesListCommand` привязан к `SelectedIndexChanged`,
  может сработать раньше обновления `SelectedBloger`.
- **Нарушенное требование:** FR-8.
- **Предлагаемое исправление:** перенести логику в setter свойства
  `SelectedBloger` (partial method `OnSelectedBlogerChanged`).

### ISSUE-8: зависимость `DbInitializer` от DI
- **Где:** `Blogers.Application/DbInitializer.cs`.
- **Проблема:** Application-слой зависит от `Microsoft.Extensions.DependencyInjection`
  через `IServiceProvider`.
- **Нарушенное требование:** NFR-4.
- **Предлагаемое исправление:** перенести `DbInitializer` в UI/Composition Root,
  передавать `IUnitOfWork` напрямую.

### ISSUE-9: лишняя зависимость `Abp.Domain.Uow`
- **Где:** `Blogers.Persistense/Repository/EfUnitOfWork.cs`.
- **Проблема:** подключён `using Abp.Domain.Uow;`, не относящийся к проекту.
- **Нарушенное требование:** NFR-4.
- **Предлагаемое исправление:** удалить `using` и алиас `IUnitOfWork`.

### ISSUE-10: Singleton `DbContext` и `IUnitOfWork`
- **Где:** `Blogers.Persistense/DependencyInjection.cs`.
- **Проблема:** `AddSingleton<IUnitOfWork, EfUnitOfWork>()` и
  `AddSingleton<AppDbContext>(...)`. `DbContext` не потокобезопасен.
- **Нарушенное требование:** NFR-3, NFR-4.
- **Предлагаемое исправление:** `AddScoped` (или `AddTransient` для MAUI).

### ISSUE-11: второй `ServiceProvider` в `MauiProgram`
- **Где:** `Blogers.UI/MauiProgram.cs`.
- **Проблема:** `builder.Services.BuildServiceProvider().Wait()` создаёт второй
  контейнер DI параллельно с основным.
- **Нарушенное требование:** NFR-4.
- **Предлагаемое исправление:** вынести seed в `IHostedService` /
  `IMauiInitializeService`.

### ISSUE-12: `EnsureCreated()` в конструкторе `AppDbContext`
- **Где:** `Blogers.Persistense/Data/AppDbContext.cs`.
- **Проблема:** побочный эффект при создании контекста.
- **Нарушенное требование:** NFR-3.
- **Предлагаемое исправление:** убрать из конструктора, вызывать в composition root.

### ISSUE-13: опечатка в XAML
- **Где:** `BlogersPage.xaml`.
- **Проблема:** «Добавить иторию» → «Добавить историю».
- **Серьёзность:** косметика.
- **Предлагаемое исправление:** исправить текст.

### ISSUE-14: `LikeToColorValueConverter` не защищён от `null`
- **Где:** `Blogers.UI/ValueConverters/LikeToColorValueConverter.cs`.
- **Проблема:** `(int)value` бросает `NullReferenceException` при `value == null`.
- **Нарушенное требование:** FR-22.
- **Предлагаемое исправление:** `if (value is int i && i < 10) ...`.

## Закрытые дефекты

_(пусто)_