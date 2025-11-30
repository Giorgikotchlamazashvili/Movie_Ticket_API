using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie_Ticket.DTOs;
using Movie_Ticket.Helpers;
using Movie_Ticket.Models;

namespace Movie_Ticket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private readonly MovieTicketContext context;
        private readonly EmailService emailService;
        private readonly TokenService tokenService;
        public UsersController(MovieTicketContext context, EmailService emailService, IConfiguration configuration, TokenService tokenService)
        {
            this.context = context;
            this.emailService = emailService;
            this.tokenService = tokenService;
        }
        [HttpPost("register")]
        public IActionResult Register(RegisterUser dto)
        {
            var existingUser = context.Users.FirstOrDefault(u => u.Email == dto.Email);

            if (existingUser != null)
            {
                return BadRequest("Email already exists.");
            }
            string emailPattern = @"^[\w\.-]+@[\w\.-]+\.\w{2,}$";
            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";

            if (!Regex.IsMatch(dto.Email, emailPattern))
            {
                return BadRequest("Email format is invalid");
            }
            if (!Regex.IsMatch(dto.Password, passwordPattern))
            {
                return BadRequest("Password must contain uppercase, lowercase, number, and be at least 8 characters");
            }
            string hashed = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = hashed,
                RefreshToken = RefreshTokenCreator.CreateRefreshToken(),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)

            };

            context.Users.Add(user);
            context.SaveChanges();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser dto)
        {
            var existingUser = context.Users.FirstOrDefault(u => u.Email == dto.Email);

            if (existingUser == null)
            {
                return BadRequest("Invalid email or password.");
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password, existingUser.PasswordHash);

            if (!isValid)
            {
                return BadRequest("Invalid email or password.");
            }

            Random random = new Random();
            string code = random.Next(1000, 99999).ToString();
            var verification = new EmailVerification
            {
                UserId = existingUser.UserId,
                Code = code,
                Expiration = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            context.EmailVerifications.Add(verification);
            context.SaveChanges();

            emailService.SendEmail(existingUser.Email, "Your Verification Code and ID is", $"code : {code} id : {existingUser.UserId}");

            return Ok(new
            {
                Message = "Verification code sent.",
                UserId = existingUser.UserId
            });



        }
        [HttpPost("verify")]
        public IActionResult Verify(VerifyCodeDto dto)
        {
            var record = context.EmailVerifications
                .OrderByDescending(v => v.CreatedAt)
                .FirstOrDefault(v => v.UserId == dto.UserId && v.Code == dto.Code);

            if (record == null)
            {
                return BadRequest("Invalid code.");
            }

            if (record.IsUsed == true)
            {
                return BadRequest("Code already used.");
            }

            if (record.Expiration < DateTime.UtcNow)
            {
                return BadRequest("Code expired.");
            }

            record.IsUsed = true;

            var user = context.Users.First(x => x.UserId == dto.UserId);
            user.IsVerified = true;

            context.SaveChanges();
            var tokenValue = tokenService.GenerateAccessToken(user.Email);
            return Ok(new
            {
                Token = tokenValue,
                User = new
                {
                    Id = user.UserId,
                    Email = user.Email
                },
                RefreshToken = user.RefreshToken
            });
        }

        [HttpPost("refresh")]
        public IActionResult RefreshToken(string refreshToken)
        {
            var user = context.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                return BadRequest("Invalid or expired refresh token.");

            string newAccessToken = tokenService.GenerateAccessToken(user.Email);

            string newRefreshToken = RefreshTokenCreator.CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            context.SaveChanges();

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }


        [Authorize]
        [HttpGet("me")]
        public IActionResult GetUser()
        {
            var emailClaim = User.FindFirst("Email")?.Value;
            if (emailClaim == null)
                return Unauthorized();

            var user = context.Users.FirstOrDefault(u => u.Email == emailClaim);
            if (user == null)
                return NotFound();

            return Ok(new
            {
                Id = user.UserId,
                Email = user.Email,
            });
        }




    }
}
