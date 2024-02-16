namespace OnlineShop.Domain.Models;

public class Item(Guid id, string title, string description, int price, int amount)
{
    public Guid Id { get; set; } = id;
    
    public string Title { get; set; } = title;
    
    public string Description { get; set; } = description;
    
    public int Price { get; set; } = price;

    public int Amount { get; set; } = amount;
};