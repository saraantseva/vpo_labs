
using Blogers.UI.ViewModels;

namespace Blogers.UI.Pages;

public partial class BlogersPage : ContentPage
{
    private readonly BlogersPageViewModel _viewModel;
	public BlogersPage(BlogersPageViewModel viewModel)
	{
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}
