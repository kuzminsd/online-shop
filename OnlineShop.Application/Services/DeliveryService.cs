using OnlineShop.Application.Contracts;
using OnlineShop.Application.Contracts.Data;
using OnlineShop.Application.Exceptions;
using OnlineShop.Domain.Models;

namespace OnlineShop.Application.Services;

public class DeliveryService(
    IOrderService orderService,
    IDeliveryRepository deliveryRepository): IDeliveryService
{
    public async Task<Guid> Create(Guid orderId, long slot)
    {
        var deliverySlot = (long) TimeSpan.FromSeconds(slot).TotalMilliseconds;
        var delivery = await deliveryRepository.Create(orderId, deliverySlot);
        
        try
        {
           await orderService.SetDelivery(orderId, delivery.Id);
        }
        catch (InvalidProcessException)
        {
            //TODO: cancel delivery
            throw;
        }

        return delivery.Id;
    }
}