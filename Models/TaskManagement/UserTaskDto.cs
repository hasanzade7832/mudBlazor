namespace BlazorApp1.Models.TaskManagement;

public class UserTaskDto
{
    public int Id { get; set; }
    public int TaskItemId { get; set; }
    public string UserId { get; set; } = string.Empty;

    public bool IsCompletedByUser { get; set; }
    public bool IsConfirmedByAdmin { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }

    public int PercentComplete { get; set; }

    public SimpleUserDto? User { get; set; }  // اطلاعات کاربر تخصیص‌یافته
}
