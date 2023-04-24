namespace FinancioBackend.Models;

public class Budget
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Amount { get; set; }
    public List<Expense> Expenses { get; set; } = new List<Expense>();
}