using Service.Contracts;
using Tournament.Core.Repositories;
using Tournament.Data.Data;
using Tournament.Data.Repositories;
using Tournament.Services;
using Turnament.Data.Repositories;

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
            services.AddScoped<ITournamentService, TournamentService>();
            services.AddAutoMapper(typeof(TournamentMappings));

            services.AddLazy<IGameService>();
            services.AddLazy<ITournamentService>();
        }
        
        public static void ConfigureRepositoryLayerServices(this IServiceCollection services)
        {
            services.AddScoped<ITournamentRepository, TournamentRepository>();
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddLazy<IGameRepository>();
            services.AddLazy<ITournamentRepository>();
        }

        public static IServiceCollection AddLazy<TService>(this IServiceCollection services) where TService : class
        {
            return services.AddScoped(provider => new Lazy<TService>(() => provider.GetRequiredService<TService>()));
        }
    }
}
