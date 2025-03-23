using be.Repositories;
using be.Repositories.impl;
using be.Services;

namespace be.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddDependencyInjectionServices(this IServiceCollection services){
        
        services.AddScoped(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
        services.AddScoped(typeof(IRepositorySync<>), typeof(RepositorySync<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
}