using MassTransit;
using Shared.Events;

namespace LoggingService.Consumers;

public class ScheduleUpdatedConsumer : IConsumer<ScheduleUpdatedEvent>
{
    public Task Consume(ConsumeContext<ScheduleUpdatedEvent> context)
    {
        var e = context.Message;
        Console.WriteLine(
            $"[{e.Timestamp}] Train {e.TrainId}: departure {e.OldDepartureTime:t} → {e.NewDepartureTime:t}, arrival {e.OldArrivalTime:t} → {e.NewArrivalTime:t}");
        return Task.CompletedTask;
    }
}
