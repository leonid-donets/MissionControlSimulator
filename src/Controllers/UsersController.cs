// using System;
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.IdentityModel.Tokens;
// using MissionControlSimulator.src.Service;
// using MissionControlSimulator.src.Models;

// namespace MissionControlSimulator.src.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class UsersController : ControllerBase
//     {
//         private readonly UsersService _service;

//         public UsersController(UsersService service)
//         {
//             _service = service;
//         }
//         [HttpGet("deleted")]
//         [Authorize(Roles = "Admin")] // רק אדמין יכול לראות מחוקים
//         public async Task<IActionResult> GetDeletedUsers()
//         {
//             var deletedUsers = await _service.GetDeletedUsersAsync();
//             return Ok(deletedUsers);
//         }
// [HttpPost("login")]
// public async Task<IActionResult> Login([FromBody] LoginDto dto)
// {
//     var user = await _service.FindByUsernameAsync(dto.Username);
//     if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
//         return Unauthorized();

//     var claims = new[]
//     {
//         new Claim("Id", user.Id),
//         new Claim(ClaimTypes.Role, user.UserRole)
//     };

//     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("replace_with_a_long_secret_key_!"));
//     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//     var token = new JwtSecurityToken(
//         claims: claims,
//         expires: DateTime.UtcNow.AddHours(6),
//         signingCredentials: creds
//     );

//     return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
// }



//         [HttpGet]
//         [Authorize(Roles = "Admin")]
//         public async Task<IActionResult> GetAllUsers()
//         {
//             var users = await _service.GetAsync();
//             return Ok(users);
//         }

//         [HttpGet("{id}")]
//         [Authorize(Roles = "Admin")]
//         public async Task<IActionResult> GetUserById(string id)
//         {
//             var user = await _service.GetAsync(id);
//             if (user == null)
//                 return NotFound($"User with Id {id} not found.");

//             return Ok(user);
//         }

//         [HttpPost]
//         public async Task<IActionResult> CreateUser([FromBody] model.User user)
//         {
//             user.Id ??= MongoDB.Bson.ObjectId.GenerateNewId().ToString();
//             user.CreatedAt = DateTime.UtcNow;
//             user.UpdatedAt = DateTime.UtcNow;

//             // Normalize role
//             if (!string.IsNullOrEmpty(user.UserRole))
//             {
//                 var role = user.UserRole.ToLower();
//                 user.UserRole = role switch
//                 {
//                     "admin" => "Admin",
//                     "user" => "User",
//                     _ => "User"
//                 };
//             }
//             else
//             {
//                 user.UserRole = "User";
//             }

//             var createdUser = await _service.CreateAsync(user);
//             return Ok(createdUser);
//         }

//         [HttpDelete("{id}")]
//         [Authorize]
//         public async Task<IActionResult> DeleteUser(string id)
//         {
//             var currentUserId = User.FindFirst("id")?.Value;
//             var currentUserRole = User.FindFirst("role")?.Value;

//             if (currentUserId == null) return Unauthorized();

//             if (currentUserId != id && currentUserRole != "Admin")
//                 return Forbid();

//             var success = await _service.SoftDeleteAsync(id);
//             if (!success) return NotFound();

//             return NoContent();
//         }

//         [HttpPut("restore/{id}")]
//         [Authorize]
//         public async Task<IActionResult> RestoreUser(string id)
//         {
//             var currentUserId = User.FindFirst("id")?.Value;
//             var currentUserRole = User.FindFirst("role")?.Value;

//             if (currentUserId == null) return Unauthorized();

//             bool isOwner = currentUserId == id;
//             var success = await _service.RestoreAsync(id, currentUserRole, isOwner);

//             if (!success) return Forbid(); // לא ניתן לשחזר
//             return NoContent();
//         }
//     }
// }


using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MissionControlSimulator.src.Models;
using MissionControlSimulator.src.Service;
using BCrypt.Net;
using MissionControlSimulator.src.model;

namespace MissionControlSimulator.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _service;
        private const string SecretKey = "replace_with_a_long_secret_key_!";

        public UsersController(UsersService service)
        {
            _service = service;
        }

        private bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _service.FindByUsernameAsync(dto.Username);
            if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized();

            var claims = new[]
            {
                new Claim("id", user.Id),
                new Claim(ClaimTypes.Role, user.UserRole)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        [HttpGet("deleted")]
        [Authorize(Roles = "Admin")]
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
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.UserRole ??= "User";
            var created = await _service.CreateAsync(user);
            return Ok(created);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var currentUserId = User.FindFirst("id")?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserId == null) return Unauthorized();
            if (currentUserId != id && currentUserRole != "Admin") return Forbid();

            var success = await _service.SoftDeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPut("restore/{id}")]
        [Authorize]
        public async Task<IActionResult> RestoreUser(string id)
        {
            var currentUserId = User.FindFirst("id")?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserId == null) return Unauthorized();
            bool isOwner = currentUserId == id;

            var success = await _service.RestoreAsync(id, currentUserRole, isOwner);
            if (!success) return Forbid();
            return NoContent();
        }
    }
}
