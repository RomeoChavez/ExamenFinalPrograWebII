using System;
using System.Collections.Generic;
using System.Linq;
using Contrato.DTO;
using ExamenFinal.Interfaces;
using Microsoft.EntityFrameworkCore;
using ExamenFinal.Data;
using ExamenFinal.Models;

namespace ExamenFinal.Services
{
    public class TesisServicio : ITesis
    {
        private readonly BaseDeDatos _context;

        public TesisServicio(BaseDeDatos context)
        {
            _context = context;
        }

        public IEnumerable<Tesis> ObtenerTodas()
        {
            return _context.tesis
                .Include(t => t.Tutor)
            .ToList();
        }

        public Tesis? ObtenerPorId(Guid id)
        {
            return _context.tesis
                .Include(t => t.Tutor)
                .FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<Tesis> Buscar(string? termino)
        {
            var query = _context.tesis
                .Include(t => t.Tutor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(termino))
            {
                var busqueda = termino.Trim().ToLower();
                query = query.Where(t =>
                    t.Titulo.ToLower().Contains(busqueda) ||
                    t.Estudiante.ToLower().Contains(busqueda) ||
                    t.Estado.ToLower().Contains(busqueda) ||
                    (t.Tutor != null && (
                        t.Tutor.Nombre.ToLower().Contains(busqueda) ||
                        t.Tutor.Especialidad.ToLower().Contains(busqueda) ||
                        t.Tutor.Correo.ToLower().Contains(busqueda)
                    ))
                );
            }

            return query.ToList();
        }

        public bool ExisteTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo)) return false;
            var tituloNormalizado = titulo.Trim().ToLower();
            return _context.tesis.Any(t => t.Titulo.ToLower() == tituloNormalizado);
        }

        public int ContarPorTutor(Guid idTutor)
        {
            return _context.tesis.Count(t => t.IdTutor == idTutor);
        }

        public Tesis Crear(TesisDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (ExisteTitulo(dto.Titulo))
            {
                throw new InvalidOperationException($"No puede duplicarse la tesis. Ya existe una tesis registrada con el título '{dto.Titulo}'.");
            }

            var tutor = _context.tutores.Find(dto.IdTutor);
            if (tutor == null)
            {
                throw new InvalidOperationException("El tutor asignado no existe en la base de datos.");
            }

            var cantidadTesis = ContarPorTutor(dto.IdTutor);
            if (cantidadTesis >= 3)
            {
                throw new InvalidOperationException($"Regla de negocio: El tutor '{tutor.Nombre}' ya tiene asignadas {cantidadTesis} tesis. Un tutor solo puede tener un máximo de 3 tesis.");
            }

            var tesis = new Tesis
            {
                Id = Guid.NewGuid(),
                Titulo = dto.Titulo.Trim(),
                Estudiante = dto.Estudiante.Trim(),
                Estado = dto.Estado.Trim(),
                Imagen = dto.Imagen,
                IdTutor = dto.IdTutor
            };

            _context.tesis.Add(tesis);
            _context.SaveChanges();

            _context.Entry(tesis).Reference(t => t.Tutor).Load();

            return tesis;
        }
    }
}
