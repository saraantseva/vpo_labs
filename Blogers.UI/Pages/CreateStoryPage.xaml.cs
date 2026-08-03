using Blogers.UI.ViewModels;

namespace Blogers.UI.Pages;

public partial class CreateStoryPage : ContentPage
{
	public CreateStoryPage(CreateStoryPageViewModel _viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel;
	}
}