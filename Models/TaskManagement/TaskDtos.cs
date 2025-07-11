namespace BlazorApp1.Models.TaskManagement;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public string CreatedByAdminId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public List<UserTaskDto> UserTasks { get; set; } = new();
}
