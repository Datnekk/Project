using be.Dtos.Booking;
using be.Dtos.Rooms;
using be.Dtos.Services;
using be.Dtos.Users;
using be.Extensions;
using be.Models;
using Final_Project.Services.Vnpay;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<Booking>("Bookings");
    builder.EntitySet<Room>("Rooms");
    builder.EntitySet<Service>("Services");
    return builder.GetEdmModel();
}

builder.Services.AddControllers()
    .AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()
        .SetMaxTop(100)
        .AddRouteComponents("api", GetEdmModel()));


// Configure services and the app
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerServices();
builder.Services.AddControllersServices();
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddIdentityServices();
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddDependencyInjectionServices();
builder.Services.AddAutoMapperServices();
builder.Services.AddCorsServices(builder.Configuration, builder.Environment);
builder.Services.AddHttpContextAccessor();
//Connect VNPay API
builder.Services.AddScoped<IVnPayService, VnPayService>();
var app = builder.Build();

// Configure the HTTP request pipeline
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCorsPolicy(builder.Environment);
app.UseSwaggerServices(builder.Environment);
app.Run();

