using FinancioBackend.Dtos;
using FinancioBackend.Models;

namespace FinancioBackend.Repositories;

public interface IBudgetRepository
{
    public Task<IEnumerable<Budget>> GetBudgets();
    public Task<IEnumerable<Budget>> GetUserBudgets(int userId);
    public Task<Budget> GetBudget(int id);
    public Task<Budget> CreateBudget(BudgetDto budget, int userId);
    public Task UpdateBudget(int id, BudgetDto budget);
    public Task DeleteBudget(int id);
}
