using OnlineShop.Domain.Models;

namespace OnlineShop.Application.Contracts;

public interface IPaymentService
{
    Task<Payment> QueuePayment(Guid orderId, int amount);

    Task ProcessQueuedPayments();
}