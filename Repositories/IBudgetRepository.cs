using FinancioBackend.Dtos;
using FinancioBackend.Models;

namespace FinancioBackend.Repositories;

public interface IBudgetRepository
{
    public Task<IEnumerable<Budget>> GetBudgets();
    public Task<Budget> GetBudget(int id);
    public Task<Budget> CreateBudget(CreateBudgetDto budget);
    public Task UpdateBudget(int id, UpdateBudgetDto budget);
    public Task DeleteBudget(int id);
}