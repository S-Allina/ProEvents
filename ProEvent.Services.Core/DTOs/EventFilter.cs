namespace ProEvent.Services.Core.DTOs
{
    public class EventFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
    }
}
