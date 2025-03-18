using be.Profiles;

namespace be.Extensions;

public static class AutoMapperExtensions
{
    public static IServiceCollection AddAutoMapperServices(this IServiceCollection services){
        services.AddAutoMapper(typeof(MappingProfile));
        return services;
    }
}