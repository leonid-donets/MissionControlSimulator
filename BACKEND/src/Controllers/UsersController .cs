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

        private bool VerifyPassword(string password, string hash) =>
            BCrypt.Net.BCrypt.Verify(password, hash);

        // [HttpPost("login")]
        // public async Task<IActionResult> Login([FromBody] LoginDto dto)
        // {
        //     var user = await _service.FindByUsernameAsync(dto.Username);
        //     if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
        //         return Unauthorized();

        //     var claims = new[]
        //     {
        //         new Claim("id", user.Id),
        //         new Claim(ClaimTypes.Role, user.UserRole)
        //     };

        //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        //     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //     var token = new JwtSecurityToken(
        //         claims: claims,
        //         expires: DateTime.UtcNow.AddHours(6),
        //         signingCredentials: creds
        //     );

        //     return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        // }
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginDto dto)
{
    // מחפשים משתמש לפי שם משתמש או מייל
    var user = await _service.FindByUsernameOrEmailAsync(dto.UsernameOrEmail);

    // אם לא קיים או הסיסמה לא תואמת
    if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
        return Unauthorized();

    // יצירת טוקן JWT עם תפקיד ו-ID
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
        public async Task<IActionResult> GetDeletedUsers() =>
            Ok(await _service.GetDeletedUsersAsync());

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers() =>
            Ok(await _service.GetAsync());

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _service.GetAsync(id);
            return user == null ? NotFound() : Ok(user);
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
            return success ? NoContent() : NotFound();
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
            return success ? NoContent() : Forbid();
        }
    }
}
