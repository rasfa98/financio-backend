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

    public async Task<Budget> CreateBudget(CreateBudgetDto budget)
    {
        var budgetQuery = "INSERT INTO budgets (name, amount) VALUES (@Name, @Amount); SELECT LAST_INSERT_ID()";
        var expenseQuery = "INSERT INTO expenses (name, amount, budget_id) VALUES (@Name, @Amount, @BudgetId); SELECT LAST_INSERT_ID()";

        using (var connection = _context.CreateConnection())
        {
            connection.Open();

            var budgetParameters = new DynamicParameters();

            budgetParameters.Add("Name", budget.Name, DbType.String);
            budgetParameters.Add("Amount", budget.Amount, DbType.Int32);

            var budgetId = await connection.QuerySingleAsync<int>(budgetQuery, budgetParameters);

            var createdBudget = new Budget
            {
                Id = budgetId,
                Name = budget.Name,
                Amount = budget.Amount,
            };

            using (var transaction = connection.BeginTransaction())
            {
                foreach (var expense in budget.Expenses)
                {
                    var expenseParameters = new DynamicParameters();

                    expenseParameters.Add("Name", expense.Name, DbType.String);
                    expenseParameters.Add("Amount", expense.Amount, DbType.Int32);
                    expenseParameters.Add("BudgetId", createdBudget.Id, DbType.Int32);

                    var expenseId = await connection.QuerySingleAsync<int>(expenseQuery, expenseParameters, transaction: transaction);

                    var createdExpense = new Expense
                    {
                        Id = expenseId,
                        Name = expense.Name,
                        Amount = expense.Amount,
                    };

                    createdBudget.Expenses.Add(createdExpense);
                }

                transaction.Commit();
            }

            return createdBudget;
        }
    }

    public async Task DeleteBudget(int id)
    {
        var query = "DELETE FROM budgets WHERE Id = @Id";

        using (var connection = _context.CreateConnection())
        {
            await connection.ExecuteAsync(query, new { id });
        }
    }

    public async Task<Budget> GetBudget(int id)
    {
        var query = "SELECT * FROM budgets LEFT JOIN expenses ON budgets.id = expenses.budget_id WHERE budgets.id = @Id";

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
            }, new { Id = id });

            return budgets.Distinct().First();
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

    public async Task UpdateBudget(int id, UpdateBudgetDto budget)
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