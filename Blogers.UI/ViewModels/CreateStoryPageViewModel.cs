using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Blogers.Application.StoryUseCases.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.UI.ViewModels
{
    [QueryProperty(nameof(BlogerId), "BlogerId")]
    public partial class CreateStoryPageViewModel: ObservableObject
    {
        private readonly IMediator _mediator;
        public CreateStoryPageViewModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ObservableProperty]
        public int _blogerId;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private int? _likesCount;

        [ObservableProperty]
        private int? _viewsCount;

        [ObservableProperty]
        private string _imagePath = string.Empty;  //  для фото

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(Description))
            {
                await Shell.Current.DisplayAlertAsync("Ошибка", "Введите описание истории", "OK");
                return;
            }

            await _mediator.Send(new AddStoryRequest(Description, LikesCount ?? 0, ViewsCount ?? 0, BlogerId));

            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        // Заглушка для выбора фото
        [RelayCommand]
        private async Task SelectImage()
        {
            // Временная заглушка
            await Shell.Current.DisplayAlertAsync("Фото", "Выбор фото будет добавлен позже", "OK");
            // TODO: добавить выбор фото
        }
    }
}
