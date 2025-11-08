namespace TrainService.Models;

public class Train
{
    public int Id { get; set; }
    public string TrainNumber { get; set; }
    public string FromStation { get; set; }
    public string ToStation { get; set; }
    public string TrainType { get; set; }
}
