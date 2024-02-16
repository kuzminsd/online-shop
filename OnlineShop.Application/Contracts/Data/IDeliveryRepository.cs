using OnlineShop.Domain.Models;

namespace OnlineShop.Application.Contracts.Data;

public interface IDeliveryRepository
{
    public Task<Delivery> Create(Guid orderId, long deliverySlot);

    public Task<Delivery> Get(Guid deliveryId);
}