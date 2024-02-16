using OnlineShop.Application.Models;

namespace OnlineShop.Application.Contracts;

public interface IUserService
{
    public Task<UserInfo> Add(string name, string password);
    
    public Task<UserInfo> Get(Guid userId);
}