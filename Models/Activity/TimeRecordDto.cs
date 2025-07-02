public class TimeRecordDto
{
    public int Id { get; set; }
    public int ActivityId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public string? Duration { get; set; }
    public string? Title { get; set; }
}
