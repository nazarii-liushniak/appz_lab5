using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleService.Data;
using Shared.Events;
using Shared.Models;

namespace ScheduleService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly ScheduleDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public ScheduleController(ScheduleDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetAll()
        {
            return await _context.Schedules.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Schedule>> GetById(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return NotFound();
            return schedule;
        }

        [HttpPost]
        public async Task<ActionResult<Schedule>> Create(Schedule schedule)
        {
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = schedule.Id }, schedule);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Schedule updated, [FromQuery] string? mode = null)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return NotFound();

            var oldDeparture = schedule.DepartureTime;
            var oldArrival = schedule.ArrivalTime;

            schedule.DepartureTime = updated.DepartureTime;
            schedule.ArrivalTime = updated.ArrivalTime;
            schedule.Status = updated.Status;

            await _context.SaveChangesAsync();

            if (mode == "event")
            {
                var evt = new ScheduleUpdatedEvent
                {
                    ScheduleId = schedule.Id,
                    TrainId = schedule.TrainId,
                    OldDepartureTime = oldDeparture,
                    NewDepartureTime = updated.DepartureTime,
                    OldArrivalTime = oldArrival,
                    NewArrivalTime = updated.ArrivalTime
                };

                await _publishEndpoint.Publish(evt);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return NotFound();
            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
