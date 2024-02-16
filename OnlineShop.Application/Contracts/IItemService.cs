using OnlineShop.Domain.Models;

namespace OnlineShop.Application.Contracts;

public interface IItemService
{
    public Task<Item> Add(Item item);
    
    public Task<Item> Get(Guid itemId);

    public Task<IEnumerable<Item>> GetAll();
    
    public Task<(HashSet<Item> bookedItems, int totalAmount)> BookItems(Dictionary<Guid, int> itemsForBooking);

    public Task Unbook(Dictionary<Guid, int> itemsForReturning);
}