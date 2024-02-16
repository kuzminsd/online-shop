using OnlineShop.Application.Contracts;
using OnlineShop.Application.Contracts.Data;
using OnlineShop.Application.Exceptions;
using OnlineShop.Application.Extensions;
using OnlineShop.Application.Models;
using OnlineShop.Domain.Models;
using OnlineShop.Domain.ValueObjects;

namespace OnlineShop.Application.Services;

public class OrderService(
    IPaymentService paymentService,
    IItemService itemService,
    IDeliveryRepository deliveryRepository,
    IOrderRepository orderRepository,
    IPaymentRepository paymentRepository) : IOrderService
{
    public async Task<OrderInfo> CreateOrder(Guid userId)
    {
        var order = await orderRepository.Create(userId);
        
        return new OrderInfo
        {
            Id = order.Id,
            UserId = order.UserId,
            TimeCreated = order.CreatedAt.ConvertToTimeMillis(),
            Status = order.OrderStatus.ConvertToString(),
            ItemsMap = order.OrderItems.ToDictionary(k => k.ItemId, v => v.Amount),
            DeliveryId = order.DeliveryId,
            DeliveryDuration = null,
            PaymentHistory = Array.Empty<PaymentInfo>()
        };
    }

    public async Task<OrderInfo> GetOrderInfo(Guid orderId)
    {
        var order = await orderRepository.Get(orderId);
        var delivery = order.DeliveryId is null ? null : await deliveryRepository.Get(order.DeliveryId.Value);
        var payment = await paymentRepository.Get(orderId);

        return new OrderInfo
        {
            Id = order.Id,
            UserId = order.UserId,
            TimeCreated = order.CreatedAt.ConvertToTimeMillis(),
            Status = order.OrderStatus.ConvertToString(),
            ItemsMap = order.OrderItems.ToDictionary(k => k.ItemId, v => v.Amount),
            DeliveryId = order.DeliveryId,
            DeliveryDuration = delivery?.Duration,
            PaymentHistory = payment.Select(x =>
                new PaymentInfo(
                    x.CreatedAt.Ticks, 
                    x.Status.ConvertToString(), 
                    x.Amount, 
                    x.Id))
        };
    }
    
    public async Task<bool> PutItemToOrder(Guid orderId, Guid itemId, int amount)
    {
        var order = await orderRepository.Get(orderId);

        if (order.OrderStatus == OrderStatus.Collecting)
        {
            return await orderRepository.PutItemToOrder(orderId, itemId, amount);
        }

        if (order.OrderStatus == OrderStatus.BookingInProgress ||
            order.OrderStatus == OrderStatus.Booked ||
            order.OrderStatus == OrderStatus.DeliverySet)
        {
            if (order.BookingId.HasValue)
            {
                await itemService.Unbook(order.OrderItems.ToDictionary(k => k.ItemId, v => v.Amount));
                await orderRepository.Unbook(orderId);
                //await bookingRepository.Unbook(order.BookingId.GetValueOrDefault());
            }
            
            return await orderRepository.PutItemToOrder(orderId, itemId, amount);
        }

        throw new InvalidProcessException($"Order {orderId} is in status {order.OrderStatus}, cannot add item");
    }

    public async Task<BookingResult> Checkout(Guid orderId)
    {
        var order = await orderRepository.Get(orderId);

        if (order.OrderStatus != OrderStatus.Collecting)
        {
            throw new InvalidProcessException(
                $"Cannot start checkout for order {orderId}, status is {order.OrderStatus}");
        }

        var bookingResult = await itemService
            .BookItems(order.OrderItems.ToDictionary(k => k.ItemId, v => v.Amount));

        var failedItems = order.OrderItems
            .Select(x => x.ItemId)
            .Where(x => bookingResult.bookedItems.All(bookedItem => bookedItem.Id != x))
            .ToHashSet();

        order = await orderRepository.BookItems(
            orderId,
            bookingResult.bookedItems.Select(x => x.Id).ToHashSet(),
            bookingResult.totalAmount);

       /* await bookingRepository.AddBooking(bookingResult.bookedItems.Select(
            x => new Booking()
            {
                Id = order.BookingId.GetValueOrDefault(),
                OrderId = orderId,
                ItemId = x.Id,
                Amount = x.Amount,
                Price = x.Price,
                Booked = true,
                CreatedAt = DateTime.UtcNow
            }));*/

        return new BookingResult(order.BookingId.GetValueOrDefault(), failedItems);
    }

    public async Task SetDelivery(Guid orderId, Guid deliveryId)
    {
        var order = await orderRepository.Get(orderId);

        if (order.OrderStatus != OrderStatus.Booked &&
            order.OrderStatus != OrderStatus.DeliverySet)
        {
            throw new InvalidProcessException(
                $"Cannot set delivery for order {orderId}, status is {order.OrderStatus}");
        }
        await orderRepository.SetDelivery(orderId, deliveryId);
    }

    public async Task<PaymentSubmissionResult> StartOrderPayment(Guid orderId)
    {
        var order = await orderRepository.Get(orderId);

        if (order.OrderStatus != OrderStatus.DeliverySet)
        {
            throw new InvalidProcessException(
                $"Order {orderId} is in status {order.OrderStatus}, cannot start payment");
        }

        if (order.BookingId is null || order.DeliveryId is null)
        {
            throw new InvalidProcessException(
                $"Order {orderId} is in status {order.OrderStatus}, cannot start payment");
        }

        await orderRepository.UpdateStatus(orderId, OrderStatus.PaymentInProgress);
       
        var result = await paymentService.StartPayment(orderId, order.TotalPrice);

        return new PaymentSubmissionResult(result.CreatedAt.Ticks, result.Id);
    }

    public async Task<IEnumerable<UserAccountFinancialLog>> GetOrderHistory(Guid orderId)
    {
        var paymentsHistory = await paymentRepository.Get(orderId);

        return paymentsHistory.Select(x => new UserAccountFinancialLog()
        {
            OrderId = orderId,
            PaymentId = x.Id,
            Timestamp = x.CreatedAt.ConvertToTimeMillis(),
            Amount = x.Amount,
        });
    }

    public async Task<IEnumerable<BookingInfo>> GetBooking(Guid bookingId)
    {
        var order = await orderRepository.GetByBookingId(bookingId);

        return order.OrderItems.Select(x => new BookingInfo()
        {
            BookingId = bookingId,
            Timestamp = order.CreatedAt.ConvertToTimeMillis(),
            ItemId = x.ItemId,
            Amount = x.Amount,
            Status = "SUCCESS"
        });
    }
}