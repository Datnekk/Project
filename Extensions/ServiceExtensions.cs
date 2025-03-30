using be.Repositories;
using be.Services;

namespace be.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IEmailService, EmailService>();  
            services.AddScoped<IOtpService, OtpService>();      

            return services;
        }
    }
}
