using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SessionMaster.DAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SessionMasterContext>(
                options =>
                {
                    string connectionString = configuration.GetConnectionString("SessionMasterContext");
                    options.UseSqlServer(connectionString);
                });

            return services;
        }
    }
}