namespace BlazorApp1.Models.Attendance
{
    public class CreateTimeEntryRequest
    {
        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string Duration { get; set; }
        public string Tasks { get; set; }
        public string ShamsiDate { get; set; }
    }
}
