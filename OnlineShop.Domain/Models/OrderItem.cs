using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Domain.Models;

[PrimaryKey(nameof(OrderId), new []{ nameof(ItemId)})]
public class OrderItem
{
    public Guid OrderId { get; set; }
    
    public Guid ItemId { get; set; }
    
    public int Amount { get; set; }
}