using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MissionControlSimulator.src.model;
using MissionControlSimulator.src.Service; // שם עקבי עם Service

namespace MissionControlSimulator.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AircraftController : ControllerBase
    {
        private readonly AircraftService _service = new AircraftService();

        // GET: api/aircraft
        [HttpGet]
        public IActionResult GetAllAircrafts()
        {
            var aircrafts = _service.GetAll();
            return Ok(aircrafts);
        }

        // GET: api/aircraft/deleted
        [HttpGet("deleted")]
        public IActionResult GetDeletedAircrafts()
        {
            var deleted = _service.GetDeleted();
            return Ok(deleted);
        }

        // GET: api/aircraft/{id}
        [HttpGet("{id}")]
        public IActionResult GetAircraftById(string id)
        {
            var aircraft = _service.GetById(id);
            if (aircraft == null)
                return NotFound();
            return Ok(aircraft);
        }

        // POST: api/aircraft
        [HttpPost]
        public IActionResult CreateAircraft([FromBody] Aircraft aircraft)
        {
            var created = _service.Create(aircraft);
            return Ok(created);
        }

        // PUT: api/aircraft/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateAircraft(string id, [FromBody] Aircraft aircraft)
        {
            var success = _service.Update(id, aircraft);
            if (!success)
                return NotFound();
            return NoContent();
        }

        // DELETE: api/aircraft/{id} - מחיקה רכה
        [HttpDelete("{id}")]
        public IActionResult DeleteAircraft(string id)
        {
            var success = _service.SoftDelete(id);
            if (!success)
                return NotFound();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Aircraft with Id {id} was soft-deleted successfully.");
            Console.ResetColor();

            return NoContent();
        }

        // PUT: api/aircraft/restore/{id} - שחזור מטוס שנמחק רך
        [HttpPut("restore/{id}")]
        public IActionResult RestoreAircraft(string id)
        {
            var success = _service.Restore(id);
            if (!success)
                return NotFound();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Aircraft with Id {id} was restored successfully.");
            Console.ResetColor();

            return NoContent();
        }
    }
}
