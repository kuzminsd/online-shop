namespace OnlineShop.Application.Models;

public class BookingInfo
{
    public Guid BookingId { get; set; }
    
    public Guid ItemId { get; set; }
    
    public string Status { get; set; }
    
    public int Amount { get; set; }
    
    public long Timestamp { get; set; }
}