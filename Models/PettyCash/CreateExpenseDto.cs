public class CreateExpenseDto
{
    public string Date { get; set; }
    public string Description { get; set; }
    public long Amount { get; set; }
    public string? ReceiptUrl { get; set; }
}
