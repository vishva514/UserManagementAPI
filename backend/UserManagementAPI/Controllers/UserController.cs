using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using UserManagementAPI.Services;
using UserManagementAPI.Data;
namespace UserManagementAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserRepository _userRepository;
    private readonly AuthService _authService;

    public UsersController(UserRepository userRepository, AuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    // DTO classes:
    public class UserRegisterDto
    {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Designation { get; set; } // "Teacher" or "Student"
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserUpdateDto
    {
        public string Email { get; set; } // Email used to identify user
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Password { get; set; }
    }

    // POST: api/users/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
        if (existingUser != null)
            return BadRequest("Email is already registered.");

        if (dto.Designation != "Teacher" && dto.Designation != "Student")
            return BadRequest("Designation must be either 'Teacher' or 'Student'.");

        var user = new User
        {
            Name = dto.Name,
            DateOfBirth = dto.DateOfBirth,
            Designation = dto.Designation,
            Email = dto.Email,
            PasswordHash = _authService.HashPassword(dto.Password)
        };

        await _userRepository.AddUserAsync(user);
        return Ok("User registration successful.");
    }

    // POST: api/users/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userRepository.GetUserByEmailAsync(dto.Email);
        if (user == null)
            return Unauthorized("Invalid email or password.");

        if (!_authService.VerifyPassword(dto.Password, user.PasswordHash))
            return Unauthorized("Invalid email or password.");

        var token = _authService.GenerateJwtToken(user);

        return Ok(new
        {
            Token = token,
            Role = user.Designation,
            Name = user.Name,
            Email = user.Email
        });
    }

    // GET: api/users
    [HttpGet]
    [Authorize(Roles = "Teacher,Student")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userRepository.GetAllUsersAsync();
        return Ok(users.Select(u => new {
            u.Id,
            u.Name,
            u.DateOfBirth,
            u.Designation,
            u.Email
        }));
    }

    // PUT: api/users
    [HttpPut]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto dto)
    {
        var user = await _userRepository.GetUserByEmailAsync(dto.Email);
        if (user == null)
            return NotFound("User not found.");

        user.Name = dto.Name;
        user.DateOfBirth = dto.DateOfBirth;
        if (!string.IsNullOrEmpty(dto.Password))
        {
            user.PasswordHash = _authService.HashPassword(dto.Password);
        }

        await _userRepository.UpdateUserAsync(user);
        return Ok("User updated successfully.");
    }

    // DELETE: api/users/{email}
    [HttpDelete("{email}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> DeleteUser(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null)
            return NotFound("User not found.");

        await _userRepository.DeleteUserAsync(email);
        return Ok("User deleted successfully.");
    }
}
