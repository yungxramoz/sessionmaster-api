using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using SessionMaster.BLL.Core;
using SessionMaster.BLL.ModAnonymousUser;
using SessionMaster.BLL.ModBoardGame;
using SessionMaster.BLL.ModSession;
using SessionMaster.BLL.ModSessionplan;
using SessionMaster.BLL.ModUser;

namespace SessionMaster.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAnonymousUserRepository, AnonymousUserRepository>();
            services.AddScoped<IBoardGameRepository, BoardGameAtlasApiRepository>();
            services.AddScoped<ISessionplanRepository, SessionplanRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IRestClient, RestClient>();

            return services;
        }
    }
}