using Microsoft.Extensions.Logging;
using OnlineShop.Application.Clients;
using OnlineShop.Application.Contracts;
using OnlineShop.Application.Contracts.Data;
using OnlineShop.Domain.Models;
using OnlineShop.Domain.ValueObjects;

namespace OnlineShop.Application.Services;

public class PaymentService(
    IPaymentClient paymentClient,
    IPaymentRepository paymentRepository,
    IOrderRepository orderRepository,
    ILogger<PaymentService> logger): IPaymentService
{
    public async Task<Payment> QueuePayment(Guid orderId, int amount)
    {
        var payment = await paymentRepository.Create(orderId, amount);
        logger.LogInformation("Payment {paymentId} for order {orderId} created", payment.Id, orderId);
        return payment;
    }

    public async Task ProcessQueuedPayments()
    {
        const string serviceName = "test";
        var accountName = "default-4";
        var queuedPayments = await paymentRepository.GetPaymentsForProcessing();

        foreach (var payment in queuedPayments)
        {
            var paymentId = payment.Id;
            
            try
            {
                logger.LogInformation("[{AccountName}] Submitting payment request for payment {PaymentId}", 
                    accountName,
                    paymentId);

                using var response = await paymentClient.Process(serviceName, accountName, paymentId); 
                await paymentRepository.FinishPayment(paymentId, response.IsSuccessStatusCode);
                await orderRepository.UpdateStatus(payment.OrderId, OrderStatus.Payed);
                logger.LogInformation("[{AccountName}] Submission finished for payment {PaymentId}: {Result}",
                    accountName,
                    paymentId,
                    response.IsSuccessStatusCode);

            }
            catch (Exception ex)
            {
                logger.LogError("[{AccountName}] Submission failed for payment {PaymentId}: {ErrorMessage}",
                    accountName, 
                    paymentId,
                    ex.Message);
            
                await paymentRepository.FinishPayment(paymentId, false);
            }
        }
    }
}