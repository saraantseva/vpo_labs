using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Blogers.Application.StoryUseCases.Commands;
using Blogers.UI.Pages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.UI.ViewModels
{
    [QueryProperty(nameof(Story), "story")]
    public partial class StoryDetailsViewModel : ObservableObject
    {
        private IMediator _mediator;
        public StoryDetailsViewModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ObservableProperty]
        private Story _story;

        [RelayCommand]
        async void ChangeStory(Story story) => await GotoChangeStoryPage(story);
        public async Task GotoChangeStoryPage(Story story)
        {
            var parametrs = new Dictionary<string, object>
            {
                {"story", story },
            };
            await Shell.Current.GoToAsync(nameof(ChangeStoryPage), parametrs);
        }

        [RelayCommand]
        private async Task DeleteStory()
        {
            bool confirm = await Shell.Current.DisplayAlertAsync(
                "Подтверждение удаления",
                $"Вы уверены, что хотите удалить историю \"{Story.Description}\"?",
                "Да, удалить",
                "Отмена");

            if (confirm)
            {
                var imagesFolder = Path.Combine(FileSystem.AppDataDirectory, "Images");
                var imagePath = Path.Combine(imagesFolder, $"{Story.Id}.jpg");
                if (File.Exists(imagePath))
                    File.Delete(imagePath);

                await _mediator.Send(new DeleteStoryCommand(Story.Id));
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        private async Task AddPhoto()
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Выберите изображение",
                FileTypes = FilePickerFileType.Images
            });

            if (result == null) return;

            var imagesFolder = Path.Combine(FileSystem.AppDataDirectory, "Images");
            Directory.CreateDirectory(imagesFolder);

            var destPath = Path.Combine(imagesFolder, $"{Story.Id}.jpg");

            using var sourceStream = await result.OpenReadAsync();
            using var destStream = File.Create(destPath);
            await sourceStream.CopyToAsync(destStream);

            OnPropertyChanged(nameof(Story));
        }

    }
}
