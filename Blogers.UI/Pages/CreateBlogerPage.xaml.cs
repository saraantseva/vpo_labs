using Blogers.UI.ViewModels;

namespace Blogers.UI.Pages;

public partial class CreateBlogerPage : ContentPage
{
	public CreateBlogerPage(CreateBlogerPageViewModel ViewModels)
	{
		InitializeComponent();
		BindingContext = ViewModels;
	}
}