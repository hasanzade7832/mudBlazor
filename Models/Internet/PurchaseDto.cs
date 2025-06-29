namespace BlazorApp1.Models;

public class PurchaseDto
{
    public int Id { get; set; }
    public string Description { get; set; }
    public double Volume { get; set; }
    public double Amount { get; set; }
    public List<DownloadDto> Downloads { get; set; } = new();
}
