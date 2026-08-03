using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Blogers.Application.BlogerUseCases.Queries;
using Blogers.Application.StoryUseCases.Commands;
using Blogers.UI.ValueConverters;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Blogers.UI.ViewModels
{
    [QueryProperty(nameof(Story), "story")]
    public partial class ChangeStoryPageViewModel : ObservableObject
    {
        private readonly IMediator _mediator;
        private byte[] _selectedImageBytes = null;
        private string _selectedImagePath = string.Empty;

        public ChangeStoryPageViewModel(IMediator mediator)
        {
            _mediator = mediator;
            Blogers = new ObservableCollection<Bloger>();
        }

        [ObservableProperty]
        private Story _story = null!;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private int? _likesCount;

        [ObservableProperty]
        private int? _viewsCount;

        public ObservableCollection<Bloger> Blogers { get; set; }

        [ObservableProperty]
        private Bloger? _selectedBloger;

        [ObservableProperty]
        private ImageSource _displayImage = null!;

        [RelayCommand]
        public async Task LoadData()
        {
            if (Story != null)
            {
                Description = Story.Description;
                LikesCount = Story.LikesCount;
                ViewsCount = Story.ViewCount;

                // Загружаем существующее фото через конвертер
                var converter = new IdToImageConverter();
                DisplayImage = (ImageSource)converter.Convert(Story.Id, typeof(ImageSource), null, null);

                await LoadBlogers(Story.BlogerId);
            }
        }

        private async Task LoadBlogers(int? currentBlogerId)
        {
            var blogers = await _mediator.Send(new GetAllBlogersRequest());

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Blogers.Clear();
                foreach (var bloger in blogers)
                {
                    Blogers.Add(bloger);
                }

                if (currentBlogerId.HasValue)
                {
                    SelectedBloger = Blogers.FirstOrDefault(b => b.Id == currentBlogerId.Value);
                }
            });
        }

        [RelayCommand]
        private async Task SelectImage()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Выберите изображение",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    _selectedImagePath = result.FullPath;

                    // Загружаем фото в память для предпросмотра
                    using var stream = await result.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    _selectedImageBytes = memoryStream.ToArray();

                    // Показываем предпросмотр
                    DisplayImage = ImageSource.FromStream(() => new MemoryStream(_selectedImageBytes));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка выбора фото: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(Description))
            {
                await Shell.Current.DisplayAlertAsync("Ошибка", "Введите описание", "OK");
                return;
            }
            if (SelectedBloger == null)
            {
                await Shell.Current.DisplayAlertAsync("Ошибка", "Выберите блогера", "OK");
                return;
            }

            // Сохраняем новое фото, если выбрано
            if (_selectedImageBytes != null)
            {
                var imagesFolder = Path.Combine(FileSystem.AppDataDirectory, "Images");
                Directory.CreateDirectory(imagesFolder);
                var destPath = Path.Combine(imagesFolder, $"{Story.Id}.jpg");

                if (File.Exists(destPath))
                    File.Delete(destPath);

                using var fileStream = File.Create(destPath);
                await fileStream.WriteAsync(_selectedImageBytes);

                // Очищаем после сохранения
                _selectedImageBytes = null;
            }

            var command = new EditStoryRequest(Story.Id, Description,
                               LikesCount ?? 0, ViewsCount ?? 0, SelectedBloger.Id);
            await _mediator.Send(command);

            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task Cancel()
        {
            // Просто возвращаемся, ничего не сохраняя
            await Shell.Current.GoToAsync("..");
        }
    }
}