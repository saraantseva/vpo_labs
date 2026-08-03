using Blogers.UI.ViewModels;

namespace Blogers.UI.Pages;

public partial class ChangeStoryPage : ContentPage
{
    private readonly ChangeStoryPageViewModel _viewModel;

    public ChangeStoryPage(ChangeStoryPageViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.LoadData();

    }
}