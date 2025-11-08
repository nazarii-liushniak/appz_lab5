using Microsoft.EntityFrameworkCore;
using ScheduleService.Models;

namespace ScheduleService.Data;

public class ScheduleDbContext(DbContextOptions<ScheduleDbContext> options) : DbContext(options)
{
    public DbSet<Schedule> Schedules { get; set; }
}
