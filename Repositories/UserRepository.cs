using System.Data;
using Dapper;
using FinancioBackend.Context;
using FinancioBackend.Models;

namespace FinancioBackend.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DapperContext _context;
    public UserRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task CreateUser(User user)
    {
        var query = "INSERT INTO users (email, password) VALUES (@Email, @Password); SELECT LAST_INSERT_ID()";

        var parameters = new DynamicParameters();

        parameters.Add("Email", user.Email, DbType.String);
        parameters.Add("Password", user.Password, DbType.String);

        using (var connection = _context.CreateConnection())
        {
            await connection.ExecuteAsync(query, parameters);
        }
    }

    public async Task<User> GetUserByEmail(string email)
    {
        var query = "SELECT * FROM users WHERE email = @Email";

        var parameters = new DynamicParameters();

        parameters.Add("Email", email, DbType.String);

        using (var connection = _context.CreateConnection())
        {
            var user = await connection.QueryFirstOrDefaultAsync<User>(query, parameters);

            return user;
        }
    }
}
