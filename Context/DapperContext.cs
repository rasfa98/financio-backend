using System.Data;
using MySqlConnector;

namespace FinancioBackend.Context;

public class DapperContext
{
    private readonly IConfiguration _configuration;
    private readonly string? _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        _configuration = configuration;
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public IDbConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}