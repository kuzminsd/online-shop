using Microsoft.EntityFrameworkCore;
using OnlineShop.Application.Contracts.Data;
using OnlineShop.Domain.Models;

namespace OnlineShop.Persistence.Repositories;

public class UserRepository(OnlineShopDbContext dbContext) : IUserRepository
{
    public async Task<User> Add(string name, string password)
    {
        var user = new User { Login = name, Password = password };
        await dbContext.Users.AddAsync(user);
        return user;
    }

    public async Task<User> Get(Guid userId)
    {
        var user = await dbContext.Users.FirstAsync(x => x.Id == userId);

        return user;
    }
}