using OnlineShop.Domain.ValueObjects;

namespace OnlineShop.Domain.Models;

public class Payment
{
    public Guid Id { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public Guid OrderId { get; set; }
    
    public int Amount { get; set; }
    
    public PaymentStatus Status { get; set; }
}