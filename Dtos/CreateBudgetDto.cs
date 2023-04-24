using System.ComponentModel.DataAnnotations;

namespace FinancioBackend.Dtos;

public class CreateBudgetDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required, Range(1, Int32.MaxValue)]
    public int Amount { get; set; }

    public List<CreateExpenseDto> Expenses { get; set; } = new List<CreateExpenseDto>();
}