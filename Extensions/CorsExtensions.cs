namespace be.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        services.AddCors(options => {
            options.AddPolicy("DefaultCorsPolicy", policy => {
              policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
            Console.WriteLine($"CORS Allowed origins: {string.Join(", ", allowedOrigins)}");
            });
        });
        return services;
    }

    public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseCors("DefaultCorsPolicy");
        }
        else
        {
            app.UseCors("DefaultCorsPolicy");
        }

        return app;
    }
}