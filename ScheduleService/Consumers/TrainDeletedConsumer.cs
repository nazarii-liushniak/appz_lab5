using MassTransit;
using ScheduleService.Data;
using Shared.Events;

namespace ScheduleService.Consumers;

public class TrainDeletedConsumer(ScheduleDbContext context) : IConsumer<TrainDeletedEvent>
{
    private readonly ScheduleDbContext _context = context;

    public Task Consume(ConsumeContext<TrainDeletedEvent> context)
    {
        var e = context.Message;
        var schedule = _context.Schedules.Find(e.TrainId);
        if (schedule == null) return Task.CompletedTask;
        _context.Schedules.Remove(schedule);
        _context.SaveChanges();

        return Task.CompletedTask;
    }
}