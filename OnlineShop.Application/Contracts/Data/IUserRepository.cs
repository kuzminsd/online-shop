using OnlineShop.Domain.Models;

namespace OnlineShop.Application.Contracts.Data;

public interface IUserRepository
{
    public Task<User> Add(string name, string password);

    public Task<User> Get(Guid userId);
}