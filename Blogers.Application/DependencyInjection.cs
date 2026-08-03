using Microsoft.Extensions.DependencyInjection;
using Blogers.Application.BlogerUseCases.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.Application
{
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
