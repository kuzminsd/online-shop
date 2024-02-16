using OnlineShop.Domain.Models;

namespace OnlineShop.Application.Contracts;

public interface IPaymentService
{
    Task<Payment> StartPayment(Guid orderId, int amount);

    Task Pay(Guid paymentId);
}