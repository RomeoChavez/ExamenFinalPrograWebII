using System;
using System.Linq;
using Contrato.DTO;
using ExamenFinal.Models;
using Microsoft.AspNetCore.Mvc;
using ExamenFinal.Interfaces;

namespace ExamenFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutorController : ControllerBase
    {
        private readonly ITutor _tutorServicio;

        public TutorController(ITutor tutorServicio)
        {
            _tutorServicio = tutorServicio;
        }

        [HttpGet(Name = "ListarTutores")]
        public IActionResult Get()
        {
            var tutores = _tutorServicio.ObtenerTodos().Select(t => new
            {
                t.Id,
                t.Nombre,
                t.Especialidad,
                t.Correo,
                TotalTesis = _tutorServicio.ContarTesis(t.Id)
            });

            return Ok(tutores);
        }

        [HttpGet("{id}", Name = "ObtenerTutorPorId")]
        public IActionResult GetById(Guid id)
        {
            var tutor = _tutorServicio.ObtenerPorId(id);
            if (tutor == null)
            {
                return NotFound(new { mensaje = $"No se encontró un tutor con el ID '{id}'." });
            }

            return Ok(new
            {
                tutor.Id,
                tutor.Nombre,
                tutor.Especialidad,
                tutor.Correo,
                TotalTesis = _tutorServicio.ContarTesis(tutor.Id)
            });
        }


        [HttpPost(Name = "GuardarTutor")]
        public IActionResult Post([FromBody] TutorDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var nuevoTutor = _tutorServicio.Crear(dto);
                return CreatedAtAction(nameof(GetById), new { id = nuevoTutor.Id }, new
                {
                    mensaje = "Tutor registrado exitosamente.",
                    tutor = nuevoTutor
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error interno al registrar el tutor.", detalle = ex.Message });
            }
        }
    }
}
