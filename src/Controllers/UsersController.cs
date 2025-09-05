using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MissionControlSimulator.src.Service;

namespace MissionControlSimulator.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _service;

        public UsersController(UsersService service)
        {
            _service = service;
        }
[HttpGet("deleted")]
[Authorize(Roles = "Admin")] // רק אדמין יכול לראות מחוקים
public async Task<IActionResult> GetDeletedUsers()
{
    var deletedUsers = await _service.GetDeletedUsersAsync();
    return Ok(deletedUsers);
}

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _service.GetAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _service.GetAsync(id);
            if (user == null)
                return NotFound($"User with Id {id} not found.");

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] model.User user)
        {
            user.Id ??= MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            // Normalize role
            if (!string.IsNullOrEmpty(user.UserRole))
            {
                var role = user.UserRole.ToLower();
                user.UserRole = role switch
                {
                    "admin" => "Admin",
                    "user" => "User",
                    _ => "User"
                };
            }
            else
            {
                user.UserRole = "User";
            }

            var createdUser = await _service.CreateAsync(user);
            return Ok(createdUser);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var currentUserId = User.FindFirst("id")?.Value;
            var currentUserRole = User.FindFirst("role")?.Value;

            if (currentUserId == null) return Unauthorized();

            if (currentUserId != id && currentUserRole != "Admin")
                return Forbid();

            var success = await _service.SoftDeleteAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpPut("restore/{id}")]
        [Authorize]
        public async Task<IActionResult> RestoreUser(string id)
        {
            var currentUserId = User.FindFirst("id")?.Value;
            var currentUserRole = User.FindFirst("role")?.Value;

            if (currentUserId == null) return Unauthorized();

            bool isOwner = currentUserId == id;
            var success = await _service.RestoreAsync(id, currentUserRole, isOwner);

            if (!success) return Forbid(); // לא ניתן לשחזר
            return NoContent();
        }
    }
}
