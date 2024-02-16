using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Application.Clients;
using OnlineShop.Application.Contracts;
using OnlineShop.Application.Contracts.Data;
using OnlineShop.Application.Services;
using OnlineShop.Domain.Models;
using OnlineShop.Host.ApiModels;
using OnlineShop.Host.ApiModels.Authentication;
using OnlineShop.Host.HostedService;
using OnlineShop.Host.Infrastructure;
using OnlineShop.Persistence;
using OnlineShop.Persistence.Repositories;
using Refit;

var builder = WebApplication.CreateBuilder(args); 

builder.WebHost.ConfigureKestrel((options) =>
{
    options.ConfigureEndpointDefaults(lo => lo.Protocols = HttpProtocols.Http2);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


ResolveDependencies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseMiddleware<LoggingMiddleware>();

ConfigureAuthApi();
ConfigureUsersApi();
ConfigureOrdersApi();
ConfigureItemsApi();

app.Run();

void ConfigureAuthApi()
{
    app.MapPost("/authentication", ([FromBody]AuthenticationRequest request) 
            => new AuthenticationResponse("accessToken", "refreshToken"))
        .WithTags("Authentication")
        .WithName("Authentication")
        .WithOpenApi();
    
    app.MapPost("/authentication/refresh", ([FromBody]AuthenticationRequest request) 
            => new AuthenticationResponse("accessToken", "refreshToken"))
        .WithTags("Authentication")
        .WithName("Authentication Refresh")
        .WithOpenApi();
}

void ConfigureUsersApi()
{
    app.MapPost("/users", (IUserService userService, [FromBody]CreateUserRequest request) 
            => userService.Add(request.Name, request.Password))
        .WithTags("Users")
        .WithName("Create User")
        .WithOpenApi();
}

void ConfigureOrdersApi()
{
    app.MapPost("/orders", (IOrderService orderService, [FromQuery] Guid userId) 
            => orderService.CreateOrder(userId))
        .WithTags("Orders")
        .WithName("Create Order")
        .WithOpenApi();
    
    app.MapGet("/orders/{orderId}", (IOrderService orderService, [FromRoute] Guid orderId) 
            => orderService.GetOrderInfo(orderId))
        .WithTags("Orders")
        .WithName("Get Order Info By Id")
        .WithOpenApi();

    app.MapPut("/orders/{orderId}/items/{itemId}",
        async (IOrderService orderService, [FromRoute] Guid orderId, [FromRoute] Guid itemId, [FromQuery] int amount)
            => await orderService.PutItemToOrder(orderId, itemId, amount))
        .WithTags("Orders")
        .WithName("Put Item To Order")
        .WithOpenApi();

    app.MapPost("/orders/{orderId}/bookings", async (IOrderService orderService, [FromRoute] Guid orderId) 
            => await orderService.Checkout(orderId))
        .WithTags("Orders")
        .WithName("Book Order")
        .WithOpenApi();

    app.MapGet("/orders/delivery/slots", () => new List<long> { 3, 5, 8, 10 })
        .WithTags("Orders")
        .WithName("Get Delivery Slots")
        .WithOpenApi();

    app.MapPost("/orders/{orderId}/delivery",
            async (IDeliveryService deliveryService, [FromRoute] Guid orderId, [FromQuery] long slot)
                => await deliveryService.Create(orderId, slot))
        .WithTags("Orders")
        .WithName("Set Delivery Slot")
        .WithOpenApi();
    
    app.MapPost("/orders/{orderId}/payment", 
            async (IOrderService orderService, [FromRoute] Guid orderId) 
                => await orderService.StartOrderPayment(orderId))
        .WithTags("Orders")
        .WithName("Start Payment Process")
        .WithOpenApi();
    
    app.MapGet("/orders/{orderId}/finlog", async (IOrderService orderService, [FromRoute] Guid orderId)
       => await orderService.GetOrderHistory(orderId))
        .WithTags("Orders")
        .WithName("Get Order Payment History")
        .WithOpenApi();
    
    app.MapGet("/_internal/bookingHistory/{bookingId}", async (IOrderService orderService, [FromRoute] Guid bookingId)
            => await orderService.GetBooking(bookingId))
        .WithTags("Orders")
        .WithName("Get Order Booking History")
        .WithOpenApi();
}

void ConfigureItemsApi()
{
    app.MapGet("/items", async (IItemService itemService, [FromQuery] bool available) =>
        {
            var items = await itemService.GetAll();
            
            return items.Where(x => !available  || x.Amount > 0);
        })
        .WithTags("Items")
        .WithName("Get All Items")
        .WithOpenApi();
    
    app.MapPost("/items", (IItemService itemService, [FromBody]CreateItemRequest request) 
            => itemService.Add(new Item(Guid.NewGuid(), request.Title, request.Description, request.Price, request.Amount)))
        .WithTags("Items")
        .WithName("Add Item To Catalog")
        .WithOpenApi();
    
    app.MapGet("/items/{itemId}", async (IItemService itemService, [FromQuery] Guid itemId) 
            => await itemService.Get(itemId))
        .WithTags("Items")
        .WithName("Get Item By Id")
        .WithOpenApi();
}

void ResolveDependencies()
{
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();

    builder.Services.AddScoped<IOrderService, OrderService>();
    builder.Services.AddScoped<IOrderRepository, OrderRepository>();
    
    builder.Services.AddScoped<IItemService, ItemService>();
    builder.Services.AddScoped<IItemRepository, ItemRepository>();
    
    builder.Services.AddScoped<IDeliveryService, DeliveryService>();
    builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();

    builder.Services.AddScoped<IPaymentService, PaymentService>();
    builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
    builder.Services
        .AddRefitClient<IPaymentClient>()
        .ConfigureHttpClient((_, client) =>
        {
            client.BaseAddress = new Uri("http://localhost:1234/");
        });
    
    builder.Services.AddDbContext<OnlineShopDbContext>(q => q
        .UseNpgsql(builder.Configuration.GetConnectionString("OnlineShop")));

    builder.Services.AddHostedService<PaymentHostedService>();
}

