
namespace BlazorApp1.Models;

public class PettyCashDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<ExpenseDto> ArchivedExpenses { get; set; } = new();
}
