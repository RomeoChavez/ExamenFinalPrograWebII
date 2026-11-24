using System;
using System.ComponentModel.DataAnnotations;

namespace Contrato.DTO
{
    public class TutorDTO
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "El nombre del tutor es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especialidad es obligatoria")]
        public string Especialidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        public string Correo { get; set; } = string.Empty;
    }
}
