namespace BlazorApp1.Models;

public class AttendanceDto
{
    public int Id { get; set; }
    public string Date { get; set; }
    public string Status { get; set; } // مثلا "Present", "Absent"
}
