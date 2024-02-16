using Microsoft.EntityFrameworkCore;
using OnlineShop.Application.Contracts.Data;
using OnlineShop.Domain.Models;

namespace OnlineShop.Persistence.Repositories;

public class ItemRepository(OnlineShopDbContext dbContext) : IItemRepository
{
    public async Task<Item> Add(Item item)
    {
        var savedItem = await dbContext.Items.AddAsync(item);
       
        await dbContext.SaveChangesAsync();
        
        return savedItem.Entity;
    }
    
    public Task<Item> Get(Guid itemId)
    {
       return dbContext.Items.FirstAsync(x => x.Id.Equals(itemId));
    }

    public async Task<IEnumerable<Item>> GetAll()
    {
        return await dbContext.Items.ToListAsync();
    }

    public async Task<(HashSet<Item> bookedItems, int totalAmount)> BookItems(Dictionary<Guid, int> itemsForBooking)
    {
        var totalPrice = 0;
        var bookedItems = new HashSet<Item>();
        
        foreach (var (itemId, amount) in itemsForBooking)
        {
            var item = await dbContext.Items.FirstAsync(x => x.Id == itemId);
            if (item.Amount >= amount)
            {
                item.Amount -= amount;
                totalPrice += item.Price * amount;
                bookedItems.Add(item);
            }
        }
        
        await dbContext.SaveChangesAsync();

        return (bookedItems, totalPrice);
    }

    public async Task UnbookItems(Dictionary<Guid, int> itemsForReturning)
    {
        foreach (var (itemId, amount) in itemsForReturning)
        {
            var item = await dbContext.Items.FirstAsync(x => x.Id == itemId);

            item.Amount += amount;
        }
        
        await dbContext.SaveChangesAsync();
    }
}