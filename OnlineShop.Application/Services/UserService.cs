using OnlineShop.Application.Contracts;
using OnlineShop.Application.Contracts.Data;
using OnlineShop.Application.Models;
using OnlineShop.Domain.Models;

namespace OnlineShop.Application.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<UserInfo> Add(string name, string password)
    {
        var user = await userRepository.Add(name, password);
        return new UserInfo(user.Id, user.Login);
    }

    public async Task<UserInfo> Get(Guid userId)
    { 
        var user = await userRepository.Get(userId);
        return new UserInfo(user.Id, user.Login);
    } 
}