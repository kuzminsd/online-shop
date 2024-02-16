using OnlineShop.Application.Contracts;
using OnlineShop.Application.Contracts.Data;

namespace OnlineShop.Host.HostedService;

public class PaymentHostedService(
    IServiceProvider serviceProvider,
    ILogger<IServiceProvider> logger) : BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromSeconds(1);
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
                var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
                var paymentsForProcessing = await paymentRepository.GetPaymentsForProcessing();

                foreach (var payment in paymentsForProcessing)
                {
                    await paymentService.Pay(payment.Id);
                }

                await Task.Delay(_period, stoppingToken);
            }
            catch (Exception ex) when (ex is not TaskCanceledException)
            {
                logger.LogError("Error has occured during payment process: {exception}", ex.Message);
            }
        }
    }
}