namespace OnlineShop.Domain.Models;

public class Delivery
{
    public Guid Id { get; set; }
    
    public Guid OrderId { get; set; }
    
    public long Duration { get; set; }
}