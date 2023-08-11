using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Refugio.Controllers
{
    /// <summary>
    /// Модель для входа в систему.
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Логин пользователя.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        public string? Password { get; set; }
    }

    /// <summary>
    /// Ответ после аутентификации.
    /// </summary>
    public class AuthenticatedResponse
    {
        /// <summary>
        /// Токен для аутентификации.
        /// </summary>
        public string? Token { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Выполняет вход в систему с использованием указанного логина и пароля.
        /// </summary>
        /// <param name="model">Модель входа в систему.</param>
        /// <returns>Объект IActionResult с результатом аутентификации.</returns>
        /// <exception cref="Status200OK">Успешное выполнение обратного вызова.</exception>
        /// <exception cref="Status400BadRequest">Выбрасывается, когда предоставленные аргументы недопустимы.</exception>
        /// <exception cref="Status401Unauthorized">Выбрасывается, когда аутентификация не удалась.</exception>
        [HttpPost, Route("login")]
        public IActionResult Login(LoginModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid client request");
            }

            var config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build();

            if (model.UserName == config["Admin:Login"] && model.Password == config["Admin:Password"])
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey2410"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokenOptions = new JwtSecurityToken(
                    issuer: "https://localhost:7104",
                    audience: "https://localhost:7104",
                    claims: new List<Claim>(),
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signinCredentials
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                return Ok(new { Token = tokenString });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}