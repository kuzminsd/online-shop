namespace OnlineShop.Application.Models;

public class BookingResult(Guid id, HashSet<Guid> failedItems)
{
    public Guid Id { get; set; } = id;

    public HashSet<Guid> FailedItems { get; set; } = failedItems;
}