using Microsoft.Extensions.DependencyInjection;
using SessionMaster.BLL.Core;
using SessionMaster.BLL.ModUser;

namespace SessionMaster.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
