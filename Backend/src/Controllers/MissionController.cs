using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MissionControlSimulator.src.model;
using MissionControlSimulator.src.Service;

namespace MissionControlSimulator.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MissionController : ControllerBase
    {
        private readonly MissionService _service;

        public MissionController(MissionService service)
        {
            _service = service;
        }

        // GET /api/mission
        [HttpGet]
        [Authorize(Roles = "Admin,Operator,User")]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
        {
            var list = await _service.GetAsync(includeDeleted);
            return Ok(list);
        }

        // GET /api/mission/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Operator,User")]
        public async Task<IActionResult> GetById(string id)
        {
            var mission = await _service.GetAsync(id);
            if (mission == null) return NotFound();
            if (mission.IsDeleted) return NotFound(); // or return Ok for Admin if you prefer
            return Ok(mission);
        }

        // GET /api/mission/aircraft/{aircraftId}
        [HttpGet("aircraft/{aircraftId}")]
        [Authorize(Roles = "Admin,Operator,User")]
        public async Task<IActionResult> GetByAircraft(string aircraftId)
        {
            var list = await _service.GetByAircraftAsync(aircraftId);
            return Ok(list);
        }

        // POST /api/mission
        [HttpPost]
        [Authorize(Roles = "Admin,Operator")] // מי יכול ליצור משימה
        public async Task<IActionResult> Create([FromBody] Mission mission)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _service.CreateAsync(mission);
            return Ok(created);
        }

        // PUT /api/mission/{id} - update whole mission (Admin/Operator)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Operator")]
        public async Task<IActionResult> Update(string id, [FromBody] Mission mission)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var success = await _service.UpdateAsync(id, mission);
            if (!success) return NotFound();
            return NoContent();
        }

        // PATCH-style endpoint to update status only
        // PUT /api/mission/{id}/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Operator")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] string newStatus)
        {
            // optional: validate newStatus is one of allowed values
            var allowed = new[] { "Pending", "Active", "Completed", "Cancelled" };
            if (System.Array.IndexOf(allowed, newStatus) < 0) return BadRequest("Invalid status");

            var ok = await _service.UpdateStatusAsync(id, newStatus);
            if (!ok) return NotFound();
            return NoContent();
        }

        // DELETE (soft) /api/mission/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SoftDelete(string id)
        {
            var success = await _service.SoftDeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        // PUT /api/mission/restore/{id}
        [HttpPut("restore/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Restore(string id)
        {
            var success = await _service.RestoreAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        // GET /api/mission/deleted
        [HttpGet("deleted")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDeleted()
        {
            var list = await _service.GetAsync(includeDeleted: true);
            var deleted = list.FindAll(m => m.IsDeleted);
            return Ok(deleted);
        }
       // await _hubContext.Clients.All.SendAsync("MissionUpdated", missionDto);

    }
}
