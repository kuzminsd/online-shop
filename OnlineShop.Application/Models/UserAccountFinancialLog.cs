namespace OnlineShop.Application.Models;

public class UserAccountFinancialLog
{
    public Guid OrderId { get; set; }

    public string Type { get; set; }
    
    public int Amount { get; set; }
    
    
    public Guid PaymentId { get; set; }
    
    public long Timestamp { get; set; }
}