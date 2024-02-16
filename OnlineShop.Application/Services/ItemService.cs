using OnlineShop.Application.Contracts;
using OnlineShop.Application.Contracts.Data;
using OnlineShop.Domain.Models;

namespace OnlineShop.Application.Services;

public class ItemService(IItemRepository itemRepository): IItemService
{
    public Task<Item> Add(Item item)
    {
        return itemRepository.Add(item);
    }

    public Task<Item> Get(Guid itemId)
    {
        return itemRepository.Get(itemId);
    }

    public Task<IEnumerable<Item>> GetAll()
    {
        return itemRepository.GetAll();
    }
    
    public Task<(HashSet<Item> bookedItems, int totalAmount)> BookItems(Dictionary<Guid, int> itemsForBooking)
    {
        return itemRepository.BookItems(itemsForBooking);
    }

    public Task Unbook(Dictionary<Guid, int> itemsForReturning)
    {
        return itemRepository.UnbookItems(itemsForReturning);
    }
}