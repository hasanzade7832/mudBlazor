namespace BlazorApp1.Models;

public class ExpenseDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public int Amount { get; set; }
    public string? ReceiptUrl { get; set; }
}
