using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainService.Data;
using TrainService.Models;

namespace TrainService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrainController(TrainDbContext context) : ControllerBase
{
    private readonly TrainDbContext _context = context;

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
    public async Task<ActionResult<Train>> Create(Train train)
    {
        _context.Trains.Add(train);
        await _context.SaveChangesAsync();
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
    public async Task<IActionResult> Delete(int id)
    {
        var train = await _context.Trains.FindAsync(id);
        if (train == null) return NotFound();
        _context.Trains.Remove(train);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}