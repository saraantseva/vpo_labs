using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Blogers.Application.BlogerUseCases.Commands;
using System;
using System.Collections.Generic;
using System.Text;


namespace Blogers.UI.ViewModels
{
    public partial class CreateBlogerPageViewModel : ObservableObject
    {
        private readonly IMediator _mediator;

        public CreateBlogerPageViewModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private DateTime _dateOfBirth;

        [ObservableProperty]
        private string _nickname;

        [ObservableProperty]
        private int? _followersNumber;

        [ObservableProperty]
        private DateTime _currentDate = DateTime.Now;

        [RelayCommand]
        public async Task Save()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await Shell.Current.DisplayAlertAsync("Ошибка", "Введите имя", "OK");
                return;
            }
            if(DateOfBirth == null)
            {
                await Shell.Current.DisplayAlertAsync("Ошибка", "Введите дату рождения", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Nickname))
            {
                await Shell.Current.DisplayAlertAsync("Ошибка", "Введите никнейм", "OK");
                return;
            }

            var bloger = await _mediator.Send(new AddBlogerCommand(Name, DateOfBirth, Nickname, FollowersNumber ?? 0));

            if (bloger != null) {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Ошибка", "Ошибка создания", "OK");
            }

            
        }

        [RelayCommand]
        async Task Cancel() {
            await Shell.Current.GoToAsync("..");
        }

    }
}
