namespace FinancioBackend.Models;

public class Expense
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Amount { get; set; }
}