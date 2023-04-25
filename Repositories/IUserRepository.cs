using FinancioBackend.Dtos;
using FinancioBackend.Models;

namespace FinancioBackend.Repositories;

public interface IUserRepository
{
    public Task<User> GetUserByEmail(string email);
    public Task CreateUser(User user);
}
