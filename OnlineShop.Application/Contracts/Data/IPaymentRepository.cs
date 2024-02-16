using OnlineShop.Domain.Models;

namespace OnlineShop.Application.Contracts.Data;

public interface IPaymentRepository
{
    Task<Payment> Create(Guid orderId, int amount);

    Task<IEnumerable<Payment>> Get(Guid orderId);

    Task<IEnumerable<Payment>> GetPaymentsForProcessing();

    Task<Payment> StartPayment(Guid paymentId);

    Task<Payment> FinishPayment(Guid paymentId, bool isSuccess);
}