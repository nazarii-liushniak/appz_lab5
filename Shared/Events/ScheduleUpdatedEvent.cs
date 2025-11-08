namespace Shared.Events;

public class ScheduleUpdatedEvent
{
    public int ScheduleId { get; set; }
    public int TrainId { get; set; }
    public DateTime OldDepartureTime { get; set; }
    public DateTime NewDepartureTime { get; set; }
    public DateTime OldArrivalTime { get; set; }
    public DateTime NewArrivalTime { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
