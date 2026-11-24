using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Contrato.DTO;
using ExamenFinal.Data;
using ExamenFinal.Interfaces;
using ExamenFinal.Models;
using Microsoft.IdentityModel.Tokens;

namespace ExamenFinal.Services
{
    public class AuthServicio : IAuthInterface
    {
        private readonly BaseDeDatos _context;
        private readonly IConfiguration _configuration;

        public AuthServicio(BaseDeDatos context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public void Registrar(LoginDTO dto)
        {
            var existe = _context.usuarios.Any(u => u.NombreUsuario == dto.NombreUsuario);
            if (existe)
                throw new InvalidOperationException("El nombre de usuario ya existe.");

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                NombreUsuario = dto.NombreUsuario,
                PasswordHash = HashPassword(dto.Password),
                Rol = "Usuario"
            };

            _context.usuarios.Add(usuario);
            _context.SaveChanges();
        }

        public LoginRespuestaDTO? Login(LoginDTO dto)
        {
            var usuario = _context.usuarios.FirstOrDefault(u => u.NombreUsuario == dto.NombreUsuario);
            if (usuario == null || !VerifyPassword(dto.Password, usuario.PasswordHash))
                return null;

            var jwtSection = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);
            var expira = DateTime.UtcNow.AddMinutes(double.Parse(jwtSection["ExpireMinutes"] ?? "60"));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: expira,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            return new LoginRespuestaDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol,
                Expira = expira
            };
        }

        private static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        private static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] expectedHash = Convert.FromBase64String(parts[1]);
            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}