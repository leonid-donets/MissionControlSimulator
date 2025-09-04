using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MissionControlSimulator.src.model;
using MissionControlSimulator.src.Services;

namespace MissionControlSimulator.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AircraftController : ControllerBase  // <- חשוב מאוד
    {
        private readonly AircraftService _service = new AircraftService();

        [HttpGet]
        public IActionResult GetAllAircrafts() // פעולה שמחזירה את כל המטוסים / get all aircrafts
        {
            return Ok(_service.Get());
        }
[HttpPost]
[HttpPost]
public IActionResult CreateAircraft([FromBody] Aircraft aircraft)
{
    aircraft.CreatedAt = DateTime.UtcNow;
    aircraft.UpdatedAt = DateTime.UtcNow;

    _service.Create(aircraft);
    return Ok(aircraft);
}


        [HttpDelete("{id}")]
        //http://localhost:5155/api/aircraft/{68b9e432a4752224eafd4586}
public IActionResult DeleteAircraft(string id)
{
    var success = _service.Remove(id); // נניח ש־Remove מחזיר bool אם נמחק
    if (!success)
        return NotFound();

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Aircraft with Id {id} was deleted successfully.");
    Console.ResetColor();

    return NoContent();
}

    }
}
