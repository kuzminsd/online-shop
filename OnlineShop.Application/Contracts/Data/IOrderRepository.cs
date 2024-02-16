using OnlineShop.Domain.Models;
using OnlineShop.Domain.ValueObjects;

namespace OnlineShop.Application.Contracts.Data;

public interface IOrderRepository
{
    public Task<Order> Create(Guid userId);

    public Task<Order> Get(Guid orderId);

    public Task<bool> PutItemToOrder(Guid orderId, Guid itemId, int amount);

    public Task<Order> UpdateStatus(Guid orderId, OrderStatus newStatus);

    public Task<Order> BookItems(Guid orderId, HashSet<Guid> bookedItems, int totalPrice);

    public Task<Order> Unbook(Guid orderId);

    public Task<Order> SetDelivery(Guid orderId, Guid deliveryId);

    public Task<Order> GetByBookingId(Guid bookingId);
}