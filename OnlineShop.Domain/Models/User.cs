namespace OnlineShop.Domain.Models;

public class User
{
    public Guid Id { get; set; }

    public string Login { get; set; } = null!;
    
    public string Password { get; set; } = null!;
}