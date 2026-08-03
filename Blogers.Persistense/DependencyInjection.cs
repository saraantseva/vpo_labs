using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Blogers.Persistense.Data;
using Blogers.Persistense.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.Persistense
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddSingleton<IUnitOfWork, EfUnitOfWork>();
            return services;
        }

        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            DbContextOptions options)
        {
            services.AddPersistence().AddSingleton<AppDbContext>(
                    new AppDbContext((DbContextOptions<AppDbContext>)options));
            return services;
        }
    }

}
