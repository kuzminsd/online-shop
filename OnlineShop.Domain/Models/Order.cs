using OnlineShop.Domain.ValueObjects;

namespace OnlineShop.Domain.Models;

public class Order(Guid id, Guid userId, DateTime createdAt, OrderStatus orderStatus)
{
    public Guid Id { get; set; } = id;

    public Guid UserId { get; set; } = userId;

    public DateTime CreatedAt { get; set; } = createdAt;
    
    public Guid? BookingId { get; set; }

    public OrderStatus OrderStatus { get; set; } = orderStatus;

    public List<OrderItem> OrderItems { get; set; } = new();
    
    public Guid? DeliveryId { get; set; }
    
    public int TotalPrice { get; set; }
    
    public List<Payment> PaymentHistory { get; set; } = new();
}