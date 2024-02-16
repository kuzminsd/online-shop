using OnlineShop.Application.Models;
using Refit;

namespace OnlineShop.Application.Clients;

public interface IPaymentClient
{
    [Post("/external/process?serviceName={serviceName}&accountName={accountName}&transactionId=transactionId")]
    Task<IApiResponse<PaymentResponse>> Process(string serviceName, string accountName, Guid transactionId);
}