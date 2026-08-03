using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Blogers.Application.BlogerUseCases.Queries;
using Blogers.Application.StoryUseCases.Queries;
using Blogers.UI.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace Blogers.UI.ViewModels
{
    public partial class BlogersPageViewModel :  ObservableObject
    {
        private readonly IMediator _mediator;
        public BlogersPageViewModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ObservableCollection<Bloger> Blogers { get; set; } = new();
        public ObservableCollection<Story> Stories { get; set; } = new();

        [ObservableProperty]
        Bloger _selectedBloger;


        [ObservableProperty]
        private string _blogerName = string.Empty;

        [ObservableProperty]
        private string _blogerNickname = string.Empty;

        [ObservableProperty]
        private string _blogerFollowers = string.Empty;

        [ObservableProperty]
        private bool _showBlogerCard = false;


        [RelayCommand]
        async Task UpdateBlogersList() => await GetBlogersList();

        [RelayCommand]
        async Task UpdateStoriesList() => await GetStoriesList();

        [RelayCommand]
        async void ShowDetails(Story story) => await GotoDetailsPage(story);

        [RelayCommand]
        async void CreateBloger() => await GotoCreateBlogerPage();

        [RelayCommand]
        async void CreateStory() => await GotoCreateStoryPage();

        private async Task GotoCreateStoryPage()
        {
            if (SelectedBloger == null)
            {
                await Shell.Current.DisplayAlertAsync("Ошибка", "Сначала выберите блогера", "OK");
                return;
            }
            var parameters = new Dictionary<string, object>
            {
                {"BlogerId", SelectedBloger.Id },
            };
            await Shell.Current.GoToAsync(nameof(CreateStoryPage), parameters);
        }

        private async Task GotoCreateBlogerPage()
        {
            await Shell.Current.GoToAsync(nameof(CreateBlogerPage));
        }

        private async Task GotoDetailsPage(Story story)
        {
            if (story == null)
            {
                Debug.WriteLine("story == null");
                return;
            }

            var parameters = new Dictionary<string, object>
            {
                { "story", story }
            };

            await Shell.Current.GoToAsync(nameof(StoryDetailsPage), parameters);
        }

        public async Task GetBlogersList()
        {
            var blogers = await _mediator.Send(new GetAllBlogersRequest());
            
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Blogers.Clear();
                foreach (var bloger in blogers)
                {
                    Blogers.Add(bloger);
                }
            });

        }
        
        public async Task GetStoriesList()
        {
            if (SelectedBloger == null) return;

            BlogerName = SelectedBloger.PersonalData.Name;
            BlogerNickname = SelectedBloger.Nickname;
            BlogerFollowers = $"{SelectedBloger.FollowersNumber} followers";
            ShowBlogerCard = true;

            var stories = await _mediator.Send(new GetStoriesByBlogerIdRequest(SelectedBloger.Id));
            await MainThread.InvokeOnMainThreadAsync(() => { 
                Stories.Clear();
                foreach(var story in stories)
                {
                    Stories.Add(story);
                }
            });

        }

    }
}
