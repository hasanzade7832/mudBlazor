

namespace BlazorApp1.Models;

public class UserDetailsDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string? Email { get; set; }
    public List<ActivityDto> Activities { get; set; } = new();
    public List<PurchaseDto> Purchases { get; set; } = new();
    public List<EggLogDto> EggLogs { get; set; } = new();
}
