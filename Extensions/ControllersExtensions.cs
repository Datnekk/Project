using be.Filter;
using be.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

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
        )
        .AddOData(options => options
            .Select()
            .Filter()
            .OrderBy()
            .Expand()
            .Count()
            .SetMaxTop(100)
            .AddRouteComponents("api", GetEdmModel()));
        ;
        return services;
    }

    private static IEdmModel GetEdmModel()
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<Booking>("Bookings");
        builder.EntitySet<Room>("Rooms");
        builder.EntitySet<Service>("Services");
        return builder.GetEdmModel();
    }
}