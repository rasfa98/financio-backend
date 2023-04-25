using System.Data;
using Dapper;
using FinancioBackend.Context;
using FinancioBackend.Dtos;
using FinancioBackend.Models;

namespace FinancioBackend.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly DapperContext _context;
    public ExpenseRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Expense> CreateExpense(int budgetId, ExpenseDto expense)
    {
        var query = "INSERT INTO expenses (name, amount, budget_id) VALUES (@Name, @Amount, @BudgetId); SELECT LAST_INSERT_ID()";

        var parameters = new DynamicParameters();

        parameters.Add("Name", expense.Name, DbType.String);
        parameters.Add("Amount", expense.Amount, DbType.Int32);
        parameters.Add("BudgetId", budgetId, DbType.Int32);

        using (var connection = _context.CreateConnection())
        {
            var expenseId = await connection.QuerySingleAsync<int>(query, parameters);

            var createdExpense = new Expense
            {
                Id = expenseId,
                Name = expense.Name,
                Amount = expense.Amount,
            };

            return createdExpense;
        }
    }

    public async Task DeleteExpense(int id)
    {
        var query = "DELETE FROM expenses WHERE id = @Id";

        var parameters = new DynamicParameters();

        parameters.Add("Id", id, DbType.Int32);

        using (var connection = _context.CreateConnection())
        {
            await connection.ExecuteAsync(query, parameters);
        }
    }

    public async Task<Expense> GetExpense(int id)
    {
        var query = "SELECT * FROM expenses WHERE id = @Id";

        var parameters = new DynamicParameters();

        parameters.Add("Id", id, DbType.Int32);

        using (var connection = _context.CreateConnection())
        {
            var expense = await connection.QuerySingleAsync<Expense>(query, parameters);

            return expense;
        }
    }

    public async Task<IEnumerable<Expense>> GetBudgetExpenses(int budgetId)
    {
        var query = "SELECT * FROM expenses WHERE budget_id = @BudgetId";

        var parameters = new DynamicParameters();

        parameters.Add("BudgetId", budgetId, DbType.Int32);

        using (var connection = _context.CreateConnection())
        {
            var expenses = await connection.QueryAsync<Expense>(query, parameters);

            return expenses;
        }
    }

    public async Task UpdateExpense(int id, ExpenseDto expense)
    {
        var query = "UPDATE expenses SET name = @Name, Amount = @Amount WHERE id = @Id";

        var parameters = new DynamicParameters();

        parameters.Add("Id", id, DbType.Int32);
        parameters.Add("Name", expense.Name, DbType.String);
        parameters.Add("Amount", expense.Amount, DbType.Int32);

        using (var connection = _context.CreateConnection())
        {
            await connection.ExecuteAsync(query, parameters);
        }
    }
}
