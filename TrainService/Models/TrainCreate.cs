namespace TrainService.Models;

public class TrainCreate
{
    public int Id { get; set; }
    public string TrainNumber { get; set; }
    public string FromStation { get; set; }
    public string ToStation { get; set; }
    public string TrainType { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string Status { get; set; } = "OnTime";
}