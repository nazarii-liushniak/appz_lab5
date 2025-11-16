using System.Text;
using System.Text.Json;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Shared.Models;
using TrainService.Data;
using TrainService.Models;

namespace TrainService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrainController(TrainDbContext context, IPublishEndpoint publishEndpoint, IHttpClientFactory httpClientFactory) : ControllerBase
{
    private readonly TrainDbContext _context = context;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Train>>> GetAll()
    {
        return await _context.Trains.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Train>> GetById(int id)
    {
        var train = await _context.Trains.FindAsync(id);
        if (train == null) return NotFound();
        return train;
    }

    [HttpPost]
    public async Task<ActionResult<Train>> Create(Train train, [FromQuery] string? mode = null)
    {
        _context.Trains.Add(train);
        await _context.SaveChangesAsync();

        var schedule = new Schedule
        {
            TrainId = train.Id,
            DepartureTime = DateTime.MinValue,
            ArrivalTime = DateTime.MinValue,
        };

        if (mode == "event")
        {
            await _publishEndpoint.Publish(schedule);
        }
        else
        {
            var content = new StringContent(
                JsonSerializer.Serialize(schedule),
                Encoding.UTF8,
                "application/json"
            );
            await _httpClient.PostAsync("http://scheduleservice:8080/api/schedule", content);
        }

        return CreatedAtAction(nameof(GetById), new { id = train.Id }, train);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Train train)
    {
        if (id != train.Id) return BadRequest();
        _context.Entry(train).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, string? mode = null)
    {
        var train = await _context.Trains.FindAsync(id);
        if (train == null) return NotFound();
        _context.Trains.Remove(train);
        await _context.SaveChangesAsync();

        if (mode == "event")
        {
            await _publishEndpoint.Publish(new TrainDeletedEvent { TrainId = id });
        }
        else
        {
            await _httpClient.DeleteAsync($"http://scheduleservice:8080/api/schedule/{id}");
        }

        return NoContent();
    }
}