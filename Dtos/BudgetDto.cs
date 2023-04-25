using System.ComponentModel.DataAnnotations;

namespace FinancioBackend.Dtos;

public class BudgetDto
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required, Range(1, Int32.MaxValue)]
    public int Amount { get; set; }

    public List<ExpenseDto> Expenses { get; set; } = new List<ExpenseDto>();
}