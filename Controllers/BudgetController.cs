using FinancioBackend.Dtos;
using FinancioBackend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FinancioBackend.Controllers;

[ApiController]
[Route("budgets")]
public class BudgetController : ControllerBase
{
    private readonly IBudgetRepository _budgetRepository;

    public BudgetController(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetBudgets()
    {
        var budgets = await _budgetRepository.GetBudgets();

        return Ok(budgets);
    }

    [HttpGet("{id}", Name = "GetBudget")]
    public async Task<IActionResult> GetBudget(int id)
    {
        var budget = await _budgetRepository.GetBudget(id);

        if (budget == null)
        {
            return NotFound();
        }

        return Ok(budget);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBudget(CreateBudgetDto budget)
    {
        var createdBudget = await _budgetRepository.CreateBudget(budget);

        return CreatedAtRoute("GetBudget", new { id = createdBudget.Id }, createdBudget);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBudget(int id, UpdateBudgetDto budget)
    {
        var existingBudget = await _budgetRepository.GetBudget(id);

        if (existingBudget == null)
        {
            return NotFound();
        }

        await _budgetRepository.UpdateBudget(id, budget);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBudget(int id)
    {
        var existingBudget = await _budgetRepository.GetBudget(id);

        if (existingBudget == null)
        {
            return NotFound();
        }

        await _budgetRepository.DeleteBudget(id);

        return NoContent();
    }
}
