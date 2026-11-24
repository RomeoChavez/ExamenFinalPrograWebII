using System;
using System.Collections.Generic;
using System.Linq;
using Contrato.DTO;
using ExamenFinal.Interfaces;
using ExamenFinal.Models;
using ExamenFinal.Data;


namespace ExamenFinal.Services
{
    public class TutorServicio : ITutor
    {
        private readonly BaseDeDatos _context;

        public TutorServicio(BaseDeDatos context)
        {
            _context = context;
        }

        public IEnumerable<Tutor> ObtenerTodos()
        {
            return _context.tutores.ToList();
        }

        public Tutor? ObtenerPorId(Guid id)
        {
            return _context.tutores.Find(id);
        }

        public bool ExisteCorreo(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo)) return false;
            var correoNormalizado = correo.Trim().ToLower();
            return _context.tutores.Any(t => t.Correo.ToLower() == correoNormalizado);
        }

        public int ContarTesis(Guid idTutor)
        {
            return _context.tesis.Count(t => t.IdTutor == idTutor);
        }

        public Tutor Crear(TutorDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Validación: NO PUEDE DUPLICARSE TUTOR (por correo o nombre)
            if (ExisteCorreo(dto.Correo))
            {
                throw new InvalidOperationException($"No puede duplicarse el tutor. Ya existe un tutor con el correo '{dto.Correo}'.");
            }

            var tutor = new Tutor
            {
                Id = Guid.NewGuid(),
                Nombre = dto.Nombre.Trim(),
                Especialidad = dto.Especialidad.Trim(),
                Correo = dto.Correo.Trim().ToLower()
            };

            _context.tutores.Add(tutor);
            _context.SaveChanges();

            return tutor;
        }
    }
}
