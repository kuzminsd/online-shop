namespace OnlineShop.Domain.Models;

public class Booking
{
    public Guid Id { get; set; }
    
    public Guid ItemId { get; set; }
    
    public Guid OrderId { get; set; }
    
    public int Amount { get; set; }
    
    public int Price { get; set; }
    
    public bool Booked { get; set; }
    
    public DateTime CreatedAt { get; set; }
}