using Blogers.UI.ViewModels;

namespace Blogers.UI.Pages;

public partial class StoryDetailsPage : ContentPage
{
	private readonly StoryDetailsViewModel _viewModel;
	public StoryDetailsPage(StoryDetailsViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
        BindingContext = _viewModel;
    }
}