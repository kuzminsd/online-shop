using OnlineShop.Domain.Models;

namespace OnlineShop.Application.Contracts;

public interface IDeliveryService
{
    Task<Guid> Create(Guid orderId, long slot);
}