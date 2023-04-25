using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FinancioBackend.Dtos;
using FinancioBackend.Models;
using FinancioBackend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FinancioBackend.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthController(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    private string CreatePasswordHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPasswordHash(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    private string CreateJwtToken(User user)
    {
        List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
            };
        
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto request)
    {
        string passwordHash = CreatePasswordHash(request.Password);

        var existingUser = await _userRepository.GetUserByEmail(request.Email);

        if (existingUser != null)
        {
            return Conflict();
        }

        var newUser = new User
        {
            Email = request.Email,
            Password = passwordHash,
        };

        await _userRepository.CreateUser(newUser);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto request)
    {
        var existingUser = await _userRepository.GetUserByEmail(request.Email);

        if (existingUser == null)
        {
            return BadRequest();
        }

        if (!VerifyPasswordHash(request.Password, existingUser.Password))
        {
            return BadRequest();
        }

        string token = CreateJwtToken(existingUser);

        return Ok(token);
    }
}