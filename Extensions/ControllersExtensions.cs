using be.Filter;

namespace be.Extensions;

public static class ControllersExtensions
{
    public static IServiceCollection AddControllersServices(this IServiceCollection services){
        services.AddControllers(options => {
            options.Filters.Add<ExceptionFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            }
        );
        return services;
    }
}