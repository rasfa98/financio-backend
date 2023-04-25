using FinancioBackend.Dtos;
using FinancioBackend.Models;

namespace FinancioBackend.Repositories;

public interface IExpenseRepository
{
    public Task<IEnumerable<Expense>> GetBudgetExpenses(int budgetId);
    public Task<Expense> GetExpense(int id);
    public Task<Expense> CreateExpense(int budgetId, ExpenseDto expense);
    public Task UpdateExpense(int id, ExpenseDto expense);
    public Task DeleteExpense(int id);
}
