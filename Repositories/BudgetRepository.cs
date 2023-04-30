using System.Data;
using Dapper;
using FinancioBackend.Context;
using FinancioBackend.Dtos;
using FinancioBackend.Models;

namespace FinancioBackend.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly DapperContext _context;
    public BudgetRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Budget> CreateBudget(BudgetDto budget, int userId)
    {
        var query = "INSERT INTO budgets (name, amount, user_id) VALUES (@Name, @Amount, @UserId); SELECT LAST_INSERT_ID()";

        var parameters = new DynamicParameters();

        parameters.Add("Name", budget.Name, DbType.String);
        parameters.Add("Amount", budget.Amount, DbType.Int32);
        parameters.Add("UserId", userId, DbType.Int32);

        using (var connection = _context.CreateConnection())
        {
            var budgetId = await connection.QuerySingleAsync<int>(query, parameters);

            var createdBudget = new Budget
            {
                Id = budgetId,
                Name = budget.Name,
                Amount = budget.Amount,
            };

            return createdBudget;
        }
    }

    public async Task DeleteBudget(int id)
    {
        var query = "DELETE FROM budgets WHERE Id = @Id";

        var parameters = new DynamicParameters();

        parameters.Add("Id", id, DbType.Int32);

        using (var connection = _context.CreateConnection())
        {
            await connection.ExecuteAsync(query, parameters);
        }
    }

    public async Task<Budget> GetBudget(int id)
    {
        var query = "SELECT * FROM budgets LEFT JOIN expenses ON budgets.id = expenses.budget_id WHERE budgets.id = @Id";

        var parameters = new DynamicParameters();

        parameters.Add("Id", id, DbType.Int32);

        using (var connection = _context.CreateConnection())
        {
            var budgetDictionary = new Dictionary<int, Budget>();

            var budgets = await connection.QueryAsync<Budget, Expense, Budget>(query, (budget, expense) =>
            {
                if (!budgetDictionary.TryGetValue(budget.Id, out var currentBudget))
                {
                    currentBudget = budget;
                    budgetDictionary.Add(currentBudget.Id, currentBudget);
                }

                if (expense != null)
                {
                    currentBudget.Expenses.Add(expense);
                }

                return currentBudget;
            }, parameters);

            return budgets.Distinct().FirstOrDefault();
        }
    }

    public async Task<IEnumerable<Budget>> GetBudgets()
    {
        var query = "SELECT * FROM budgets LEFT JOIN expenses ON budgets.id = expenses.budget_id";

        using (var connection = _context.CreateConnection())
        {
            var budgetDictionary = new Dictionary<int, Budget>();

            var budgets = await connection.QueryAsync<Budget, Expense, Budget>(query, (budget, expense) =>
            {
                if (!budgetDictionary.TryGetValue(budget.Id, out var currentBudget))
                {
                    currentBudget = budget;
                    budgetDictionary.Add(currentBudget.Id, currentBudget);
                }

                if (expense != null)
                {
                    currentBudget.Expenses.Add(expense);
                }

                return currentBudget;
            });

            return budgets.Distinct().ToList();
        }
    }

    public async Task<IEnumerable<Budget>> GetUserBudgets(int userId)
    {
        var query = "SELECT * FROM budgets LEFT JOIN expenses ON budgets.id = expenses.budget_id WHERE budgets.user_id = @UserId";

        var parameters = new DynamicParameters();

        parameters.Add("UserId", userId, DbType.Int32);

        using (var connection = _context.CreateConnection())
        {
            var budgetDictionary = new Dictionary<int, Budget>();

            var budgets = await connection.QueryAsync<Budget, Expense, Budget>(query, (budget, expense) =>
            {
                if (!budgetDictionary.TryGetValue(budget.Id, out var currentBudget))
                {
                    currentBudget = budget;
                    budgetDictionary.Add(currentBudget.Id, currentBudget);
                }

                if (expense != null)
                {
                    currentBudget.Expenses.Add(expense);
                }

                return currentBudget;
            }, parameters);

            return budgets.Distinct().ToList().Select(budget => {
                budget.RemainingAmount = budget.Amount - budget.Expenses.Sum(expense => expense.Amount);

                return budget;
            });
        }
    }

    public async Task UpdateBudget(int id, BudgetDto budget)
    {
        var query = "UPDATE budgets SET Name = @Name, Amount = @Amount WHERE id = @Id";

        var parameters = new DynamicParameters();

        parameters.Add("Id", id, DbType.Int32);
        parameters.Add("Name", budget.Name, DbType.String);
        parameters.Add("Amount", budget.Amount, DbType.Int32);

        using (var connection = _context.CreateConnection())
        {
            await connection.ExecuteAsync(query, parameters);
        }
    }
}
