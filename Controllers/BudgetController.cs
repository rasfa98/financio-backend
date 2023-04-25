using FinancioBackend.Dtos;
using FinancioBackend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FinancioBackend.Controllers;

[ApiController]
[Route("budgets")]
[Authorize]
public class BudgetController : ControllerBase
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IExpenseRepository _expenseRepository;

    public BudgetController(IBudgetRepository budgetRepository, IExpenseRepository expenseRepository)
    {
        _budgetRepository = budgetRepository;
        _expenseRepository = expenseRepository;
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
    public async Task<IActionResult> CreateBudget(BudgetDto budget)
    {
        // TODO: Use transactions
        var createdBudget = await _budgetRepository.CreateBudget(budget);

        foreach (var expense in budget.Expenses)
        {
            var createdExpense = await _expenseRepository.CreateExpense(createdBudget.Id, expense);

            createdBudget.Expenses.Add(createdExpense);
        }

        return CreatedAtRoute("GetBudget", new { id = createdBudget.Id }, createdBudget);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBudget(int id, BudgetDto budget)
    {
        var existingBudget = await _budgetRepository.GetBudget(id);

        if (existingBudget == null)
        {
            return NotFound();
        }

        // TODO: Use transactions
        await _budgetRepository.UpdateBudget(id, budget);

        var expensesToDelete = existingBudget.Expenses.Where(existingExpense => budget.Expenses.All(expense => expense.Id != existingExpense.Id));
        var expensesToCreate = budget.Expenses.Where(expense => expense.Id == 0);
        var expensesToUpdate = budget.Expenses.Where(expense => expense.Id > 0);

        foreach (var expense in expensesToCreate)
        {
            await _expenseRepository.CreateExpense(id, expense);
        }

        foreach (var expense in expensesToUpdate)
        {
            await _expenseRepository.UpdateExpense(expense.Id, expense);
        }

        foreach (var expense in expensesToDelete)
        {
            await _expenseRepository.DeleteExpense(expense.Id);
        }

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
