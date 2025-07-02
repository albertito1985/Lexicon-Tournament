using Service.Contracts;
using Tournament.Services;

namespace Tournament.API.Extensions
{
    public static class ServiceExtensions
    {
    }

    public static class ServiceCollectionExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(builder =>
            {
                builder.AddPolicy("AllowAll", p =>
                p.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
            });
        }
        public static void ConfigureServiceLayerServices(this IServiceCollection services)
        {
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<IGameService, GameService>();
           // services.AddScoped<ITournamentService, TournamentService>();

            services.AddLazy<IGameService>();
           // services.AddLazy<ITournamentService>();
        }
        public static IServiceCollection AddLazy<TService>(this IServiceCollection services) where TService : class
        {
            return services.AddScoped(provider => new Lazy<TService>(() => provider.GetRequiredService<TService>()));
        }
    }
}
