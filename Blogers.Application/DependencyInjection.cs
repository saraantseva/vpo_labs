using Microsoft.Extensions.DependencyInjection;
using Blogers.Application.BlogerUseCases.Queries;
using System.Diagnostics.CodeAnalysis;

namespace Blogers.Application
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this
            IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(GetAllBlogersRequest).Assembly);
            });
            return services;
        }
    }

}
