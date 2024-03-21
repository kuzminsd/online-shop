using OnlineShop.Application.Contracts;

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
                var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
                var waitingTask = Task.Delay(_period, stoppingToken);
                var paymentProcessTask = paymentService.ProcessQueuedPayments();

                await Task.WhenAll(waitingTask, paymentProcessTask);
            }
            catch (Exception ex) when (ex is not TaskCanceledException)
            {
                logger.LogError("Error has occured during payment process: {exception}", ex.Message);
            }
        }
    }
}