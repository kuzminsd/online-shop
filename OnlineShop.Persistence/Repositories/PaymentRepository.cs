using Microsoft.EntityFrameworkCore;
using OnlineShop.Application.Contracts.Data;
using OnlineShop.Domain.Models;
using OnlineShop.Domain.ValueObjects;

namespace OnlineShop.Persistence.Repositories;

public class PaymentRepository(OnlineShopDbContext dbContext): IPaymentRepository
{
    public async Task<Payment> Create(Guid orderId, int amount)
    {
        var payment = new Payment
        {
            OrderId = orderId,
            Amount = amount,
            CreatedAt = DateTime.UtcNow,
            Status = PaymentStatus.Created
        };

        await dbContext.Payments.AddAsync(payment);
        await dbContext.SaveChangesAsync();

        return payment;
    }

    public async Task<IEnumerable<Payment>> Get(Guid orderId)
    {
        return await dbContext.Payments.Where(x => x.OrderId == orderId).ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetPaymentsForProcessing()
    {
        return await dbContext.Payments.Where(x => x.Status == PaymentStatus.Created).ToListAsync();
    }

    public async Task<Payment> StartPayment(Guid paymentId)
    {
        var payment = await dbContext.Payments.FirstAsync(x => x.Id == paymentId);

        payment.Status = PaymentStatus.Submitted;
        await dbContext.SaveChangesAsync();

        return payment;
    }

    public async Task<Payment> FinishPayment(Guid paymentId, bool isSuccess)
    {
        var payment = await dbContext.Payments.FirstAsync(x => x.Id == paymentId);

        payment.Status = isSuccess ? PaymentStatus.Success : PaymentStatus.Failed;
        await dbContext.SaveChangesAsync();

        return payment;
    }
}