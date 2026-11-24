using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Contrato.DTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ExamenFinal.Interfaces;

namespace ExamenFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TesisController : ControllerBase
    {
        private readonly ITesis _tesisServicio;
        private readonly IWebHostEnvironment _env;

        public TesisController(ITesis tesisServicio, IWebHostEnvironment env)
        {
            _tesisServicio = tesisServicio;
            _env = env;
        }

        // GET: api/Tesis
        [HttpGet(Name = "ListarTesis")]
        public IActionResult Get()
        {
            var lista = _tesisServicio.ObtenerTodas().Select(t => new TesisDTO
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Estudiante = t.Estudiante,
                Estado = t.Estado,
                Imagen = t.Imagen,
                IdTutor = t.IdTutor,
                Tutor = t.Tutor != null ? new TutorDTO
                {
                    Id = t.Tutor.Id,
                    Nombre = t.Tutor.Nombre,
                    Especialidad = t.Tutor.Especialidad,
                    Correo = t.Tutor.Correo
                } : null
            });

            return Ok(lista);
        }

        [HttpGet("{id}", Name = "ObtenerTesisPorId")]
        public IActionResult GetById(Guid id)
        {
            var t = _tesisServicio.ObtenerPorId(id);
            if (t == null)
            {
                return NotFound(new { mensaje = $"No se encontró la tesis con el ID '{id}'." });
            }

            var dto = new TesisDTO
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Estudiante = t.Estudiante,
                Estado = t.Estado,
                Imagen = t.Imagen,
                IdTutor = t.IdTutor,
                Tutor = t.Tutor != null ? new TutorDTO
                {
                    Id = t.Tutor.Id,
                    Nombre = t.Tutor.Nombre,
                    Especialidad = t.Tutor.Especialidad,
                    Correo = t.Tutor.Correo
                } : null
            };

            return Ok(dto);
        }


        [HttpGet("buscar", Name = "BuscarTesisYMostrarTutor")]
        public IActionResult Buscar([FromQuery] string? termino)
        {
            var resultados = _tesisServicio.Buscar(termino).Select(t => new TesisDTO
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Estudiante = t.Estudiante,
                Estado = t.Estado,
                Imagen = t.Imagen,
                IdTutor = t.IdTutor,
                Tutor = t.Tutor != null ? new TutorDTO
                {
                    Id = t.Tutor.Id,
                    Nombre = t.Tutor.Nombre,
                    Especialidad = t.Tutor.Especialidad,
                    Correo = t.Tutor.Correo
                } : null
            });

            return Ok(resultados);
        }


        [HttpPost(Name = "GuardarTesis")]
        public async Task<IActionResult> Post([FromForm] TesisDTO dto, IFormFile? archivoImagen)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (archivoImagen != null && archivoImagen.Length > 0)
                {
                    string webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    string carpetaDestino = Path.Combine(webRoot, "imagenes", "tesis");

                    if (!Directory.Exists(carpetaDestino))
                    {
                        Directory.CreateDirectory(carpetaDestino);
                    }

                    string extension = Path.GetExtension(archivoImagen.FileName);
                    string nombreArchivo = $"{Guid.NewGuid()}{extension}";
                    string rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

                    using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        await archivoImagen.CopyToAsync(stream);
                    }

                    dto.Imagen = $"/imagenes/tesis/{nombreArchivo}";
                }

                var nuevaTesis = _tesisServicio.Crear(dto);

                return CreatedAtAction(nameof(GetById), new { id = nuevaTesis.Id }, new
                {
                    mensaje = "Tesis registrada exitosamente con su tutor asignado.",
                    tesis = new TesisDTO
                    {
                        Id = nuevaTesis.Id,
                        Titulo = nuevaTesis.Titulo,
                        Estudiante = nuevaTesis.Estudiante,
                        Estado = nuevaTesis.Estado,
                        Imagen = nuevaTesis.Imagen,
                        IdTutor = nuevaTesis.IdTutor,
                        Tutor = nuevaTesis.Tutor != null ? new TutorDTO
                        {
                            Id = nuevaTesis.Tutor.Id,
                            Nombre = nuevaTesis.Tutor.Nombre,
                            Especialidad = nuevaTesis.Tutor.Especialidad,
                            Correo = nuevaTesis.Tutor.Correo
                        } : null
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error al procesar el registro de la tesis.", detalle = ex.Message });
            }
        }
    }
}
