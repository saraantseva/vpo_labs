using Blogers.UI.Pages;
using Blogers.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Blogers.UI
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterPages(this IServiceCollection services)
        {
            services.AddTransient<BlogersPage>();
            services.AddTransient<StoryDetailsPage>();
            services.AddTransient<CreateBlogerPage>();
            services.AddTransient<CreateStoryPage>();
            services.AddTransient<ChangeStoryPage>();
            return services;
        }

        public static IServiceCollection RegisterViewModels(this IServiceCollection services)
        {
            services.AddTransient<BlogersPageViewModel>();
            services.AddTransient<StoryDetailsViewModel>();
            services.AddTransient<CreateStoryPageViewModel>();
            services.AddTransient<CreateBlogerPageViewModel>();
            services.AddTransient<ChangeStoryPageViewModel>();
            return services;
        }
    }
}
