using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TrelloClone.API.Data;
using TrelloClone.API.Models;

namespace TrelloClone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Bu e-posta adresi zaten kullanımda.");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), // BCrypt paketini kullanabilirsin veya basitçe hashleyebilirsin. Şimdilik simüle ediyoruz:
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Not: BCrypt için üstteki satır yerine test amaçlı hızlıca güvenli String hashing uygulayabilirsin.
            user.PasswordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(dto.Password)); // Test amaçlı basit encoding

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kayıt başarıyla tamamlandı." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return Unauthorized("Geçersiz e-posta veya şifre.");

            var inputPasswordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(dto.Password));
            if (user.PasswordHash != inputPasswordHash)
                return Unauthorized("Geçersiz e-posta veya şifre.");

            var token = GenerateJwtToken(user);
            return Ok(new { token = token, message = "Giriş başarılı." });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}