using System;
using System.ComponentModel.DataAnnotations;

namespace Contrato.DTO
{
    public class TesisDTO
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "El título de la tesis es obligatorio")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del estudiante es obligatorio")]
        public string Estudiante { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado de la tesis es obligatorio")]
        public string Estado { get; set; } = string.Empty;

        public string? Imagen { get; set; }

        [Required(ErrorMessage = "Debe asignar un tutor")]
        public Guid IdTutor { get; set; }

        public TutorDTO? Tutor { get; set; }
    }
}
