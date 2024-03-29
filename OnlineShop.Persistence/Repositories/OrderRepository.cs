﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineShop.Application.Contracts.Data;
using OnlineShop.Domain.Models;
using OnlineShop.Domain.ValueObjects;

namespace OnlineShop.Persistence.Repositories;

public class OrderRepository(OnlineShopDbContext dbContext, ILogger<OrderRepository> logger) : IOrderRepository
{
    public async Task<Order> Create(Guid userId)
    {
        var order = new Order(Guid.NewGuid(), userId, DateTime.UtcNow, OrderStatus.Collecting);
        await dbContext.Orders.AddAsync(order);
        await dbContext.SaveChangesAsync();
        return order;
    }

    public async Task<Order> Get(Guid orderId)
    {
        return await dbContext.Orders
            .Include(x => x.OrderItems)
            .FirstAsync(x => x.Id == orderId);
    }

    public async Task<bool> PutItemToOrder(Guid orderId, Guid itemId, int amount)
    {
        var order = await dbContext.Orders
            .Include(order => order.OrderItems)
            .FirstAsync(x => x.Id == orderId);
        
        var item = order.OrderItems.FirstOrDefault(x => x.ItemId == itemId);

        if (item is null)
        {
            item = new OrderItem { OrderId = orderId, ItemId = itemId, Amount = amount };
            order.OrderItems.Add(item);
        }
        else
        {
            //TODO: fix it
            item.Amount = amount;
        }

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Order> UpdateStatus(Guid orderId, OrderStatus newStatus)
    {
        var order = await dbContext.Orders.FirstAsync(x => x.Id == orderId);
        order.OrderStatus = newStatus;
        await dbContext.SaveChangesAsync();
        return order;
    }

    public async Task<Order> BookItems(Guid orderId, HashSet<Guid> bookedItems, int totalPrice)
    {
        var order = await dbContext.Orders
            .Include(x => x.OrderItems)
            .FirstAsync(x => x.Id == orderId);

        order.OrderStatus = OrderStatus.Booked;
        order.TotalPrice = totalPrice;
        order.BookingId = Guid.NewGuid();
        order.OrderItems.RemoveAll(x => !bookedItems.Contains(x.ItemId));
        
        await dbContext.SaveChangesAsync();
        return order;
    }
    
    public async Task<Order> Unbook(Guid orderId)
    {
        var order = await dbContext.Orders
            .Include(x => x.OrderItems)
            .FirstAsync(x => x.Id == orderId);
        
        order.OrderStatus = OrderStatus.Collecting;
        await dbContext.SaveChangesAsync();
        
        return order;
    }

    public async Task<Order> SetDelivery(Guid orderId, Guid deliveryId)
    {
        var order = await dbContext.Orders.FirstAsync(x => x.Id == orderId);
        order.DeliveryId = deliveryId;
        order.OrderStatus = OrderStatus.DeliverySet;
        await dbContext.SaveChangesAsync();
        return order;
    }

    public Task<Order> GetByBookingId(Guid bookingId)
    {
        return dbContext.Orders
            .Include(x => x.OrderItems)
            .FirstAsync(x => x.BookingId == bookingId);
    }
}