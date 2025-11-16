using MassTransit;
using ScheduleService.Data;
using Shared.Models;

namespace ScheduleService.Consumers;

public class ScheduleCreatedConsumer(ScheduleDbContext context) : IConsumer<Schedule>
{
    private readonly ScheduleDbContext _context = context;
    public Task Consume(ConsumeContext<Schedule> context)
    {
        var e = context.Message;
        _context.Schedules.Add(e);
        _context.SaveChanges();

        return Task.CompletedTask;
    }
}