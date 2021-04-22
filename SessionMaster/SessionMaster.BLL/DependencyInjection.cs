using Microsoft.Extensions.DependencyInjection;
using SessionMaster.BLL.Core;
using SessionMaster.BLL.ModUser;

namespace SessionMaster.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
