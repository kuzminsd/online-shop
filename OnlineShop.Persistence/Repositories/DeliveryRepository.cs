using Microsoft.EntityFrameworkCore;
using OnlineShop.Application.Contracts.Data;
using OnlineShop.Domain.Models;

namespace OnlineShop.Persistence.Repositories;

public class DeliveryRepository(OnlineShopDbContext dbContext): IDeliveryRepository
{
    public async Task<Delivery> Create(Guid orderId, long duration)
    {
        var delivery = new Delivery(){OrderId = orderId, Duration = duration};
        await dbContext.Deliveries.AddAsync(delivery);
        await dbContext.SaveChangesAsync();
        return delivery;
    }

    public Task<Delivery> Get(Guid deliveryId)
    {
        return dbContext.Deliveries.FirstAsync(x => x.Id == deliveryId);
    }
}