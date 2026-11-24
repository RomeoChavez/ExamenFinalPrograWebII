using Contrato.DTO;
using ExamenFinal.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExamenFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthInterface _authServicio;

        public AuthController(IAuthInterface authServicio)
        {
            _authServicio = authServicio;
        }

        [HttpPost("registrar")]
        public IActionResult Registrar([FromBody] LoginDTO dto)
        {
            try
            {
                _authServicio.Registrar(dto);
                return Ok(new { mensaje = "Usuario registrado correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("login")]
        public ActionResult<LoginRespuestaDTO> Login([FromBody] LoginDTO dto)
        {
            var resultado = _authServicio.Login(dto);
            if (resultado == null)
                return Unauthorized(new { mensaje = "Credenciales inválidas." });

            return Ok(resultado);
        }
    }
}