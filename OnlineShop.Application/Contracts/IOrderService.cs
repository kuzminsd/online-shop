using OnlineShop.Application.Models;
using OnlineShop.Domain.Models;

namespace OnlineShop.Application.Contracts;

public interface IOrderService
{
    public Task<OrderInfo> CreateOrder(Guid userId);

    public Task<OrderInfo> GetOrderInfo(Guid orderId);

    public Task<bool> PutItemToOrder(Guid orderId, Guid itemId, int amount);

    public Task<BookingResult> Checkout(Guid orderId);

    public Task SetDelivery(Guid orderId, Guid deliveryId);

    public Task<PaymentSubmissionResult> StartOrderPayment(Guid orderId);

    public Task<IEnumerable<UserAccountFinancialLog>> GetOrderHistory(Guid orderId);

    public Task<IEnumerable<BookingInfo>> GetBooking(Guid bookingId);
}