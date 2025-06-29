namespace BlazorApp1.Models;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime Deadline { get; set; }
}
