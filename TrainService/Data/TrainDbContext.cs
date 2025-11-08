using Microsoft.EntityFrameworkCore;
using TrainService.Models;

namespace TrainService.Data;

public class TrainDbContext(DbContextOptions<TrainDbContext> options) : DbContext(options)
{
    public DbSet<Train> Trains { get; set; }
}