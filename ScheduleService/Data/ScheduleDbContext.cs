using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace ScheduleService.Data;

public class ScheduleDbContext(DbContextOptions<ScheduleDbContext> options) : DbContext(options)
{
    public DbSet<Schedule> Schedules { get; set; }
}
