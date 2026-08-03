using Blogers.UI.Pages;

namespace Blogers.UI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(StoryDetailsPage), typeof(StoryDetailsPage));
            Routing.RegisterRoute(nameof(CreateBlogerPage), typeof(CreateBlogerPage));
            Routing.RegisterRoute(nameof(CreateStoryPage), typeof(CreateStoryPage));
            Routing.RegisterRoute(nameof(ChangeStoryPage), typeof(ChangeStoryPage));
        }
    }
}
