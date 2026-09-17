using GestaoResiduos.Models;
using GestaoResiduos.Services;
using GestaoResiduos.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestaoResiduos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _authService = new AuthService();
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginViewModel loginViewModel)
        {
            // Validação básica
            if (string.IsNullOrEmpty(loginViewModel.Nome) || string.IsNullOrEmpty(loginViewModel.Senha))
            {
                return BadRequest("Nome e senha são obrigatórios");
            }

            var authenticatedUser = _authService.Authenticate(loginViewModel.Nome, loginViewModel.Senha);

            if (authenticatedUser == null)
            {
                return Unauthorized("Credenciais inválidas");
            }

            var token = GenerateJwtToken(authenticatedUser);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(Usuario user)
        {
            var secretKey = _configuration["Jwt:SecretKey"] ?? "f+ujXAKHk00L5jlMXo2XhAWawsOoihNP1OiAM25lLSO57+X7uBMQgwPju6yzyePi";
            var issuer = _configuration["Jwt:Issuer"] ?? "fiap";
            var expirationMinutes = double.TryParse(_configuration["Jwt:ExpiresInMinutes"], out var minutes) ? minutes : 60;

            byte[] secret = Encoding.ASCII.GetBytes(secretKey);
            var securityKey = new SymmetricSecurityKey(secret);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Nome),
                    new Claim(ClaimTypes.Role, user.Tipo ?? ""),
                    new Claim(ClaimTypes.Hash, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                Issuer = issuer,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secret),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}