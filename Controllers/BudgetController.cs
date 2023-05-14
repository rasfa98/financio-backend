using FinancioBackend.Dtos;
using FinancioBackend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FinancioBackend.Services;
using FinancioBackend.AuthorizationRequirements;

namespace FinancioBackend.Controllers;

[ApiController]
[Route("budgets")]
[Authorize]
public class BudgetController : ControllerBase
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserService _userService;
    private readonly IAuthorizationService _authorizationService;

    public BudgetController(IBudgetRepository budgetRepository, IExpenseRepository expenseRepository, IUserService userService, IAuthorizationService authorizationService)
    {
        _budgetRepository = budgetRepository;
        _expenseRepository = expenseRepository;
        _userService = userService;
        _authorizationService = authorizationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBudgets()
    {
        var userId = _userService.GetId();
        var budgets = await _budgetRepository.GetUserBudgets(userId);

        return Ok(budgets);
    }

    [HttpGet("{id}", Name = "GetBudget")]
    public async Task<IActionResult> GetBudget(int id)
    {
        var budget = await _budgetRepository.GetBudget(id);
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, budget, BudgetRequirement.ReadRequirement);

        if (budget == null || !authorizationResult.Succeeded)
        {
            return NotFound();
        }

        return Ok(budget);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBudget(BudgetDto budget)
    {
        var userId = _userService.GetId();
        // TODO: Use transactions
        var createdBudget = await _budgetRepository.CreateBudget(budget, userId);

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
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, existingBudget, BudgetRequirement.UpdateRequirement);

        if (existingBudget == null || !authorizationResult.Succeeded)
        {
            return NotFound();
        }

        // TODO: Use transactions
        await _budgetRepository.UpdateBudget(id, budget);

        var expensesToDelete = existingBudget.Expenses.Where(existingExpense => budget.Expenses.All(expense => expense.Id != existingExpense.Id));
        var expensesToUpdate = budget.Expenses.Where(expense => existingBudget.Expenses.Any(existingExpense => existingExpense.Id == expense.Id));
        var expensesToCreate = budget.Expenses.Where(expense => expense.Id == 0);

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
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, existingBudget, BudgetRequirement.DeleteRequirement);

        if (existingBudget == null || !authorizationResult.Succeeded)
        {
            return NotFound();
        }

        await _budgetRepository.DeleteBudget(id);

        return NoContent();
    }
}
