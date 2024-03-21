namespace OnlineShop.Application.Models;

public class OrderInfo
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public long TimeCreated { get; set; }

    public string? Status { get; set; } = null!;

    public Dictionary<Guid, int> ItemsMap { get; set; } = null!;
    
    public Guid? DeliveryId { get; set; }
    
    public long? DeliveryDuration { get; set; }
    
    public IEnumerable<PaymentInfo> PaymentHistory { get; set; } = null!;
}