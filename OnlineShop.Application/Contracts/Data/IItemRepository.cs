using OnlineShop.Domain.Models;

namespace OnlineShop.Application.Contracts.Data;

public interface IItemRepository
{
    public Task<Item> Add(Item item);

    /// <summary>
    /// Returns item from repository
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns>Item with such id</returns>
    /// <exception cref="InvalidOperationException">No element with such id</exception>
    public Task<Item> Get(Guid itemId);

    public Task<IEnumerable<Item>> GetAll();

    public Task<(HashSet<Item> bookedItems, int totalAmount)> BookItems(Dictionary<Guid, int> itemsForBooking);
    
    public Task UnbookItems(Dictionary<Guid, int> itemsForReturning);

}