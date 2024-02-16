using Microsoft.EntityFrameworkCore;
using OnlineShop.Domain.Models;

namespace OnlineShop.Persistence;

public class OnlineShopDbContext(DbContextOptions<OnlineShopDbContext> options) : DbContext(options)
{
    public required DbSet<User> Users { get; set; }
    public required DbSet<Item> Items { get; set; }
    public required DbSet<Order> Orders { get; set; }
    
    public required DbSet<Payment> Payments { get; set; }
    
    public required DbSet<Delivery> Deliveries { get; set; }
}