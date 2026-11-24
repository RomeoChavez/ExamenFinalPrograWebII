using ExamenFinal.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamenFinal.Models
{
    public class Tesis
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El título de la tesis es requerido")]
        [MaxLength(250)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del estudiante es requerido")]
        [MaxLength(150)]
        public string Estudiante { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado de la tesis es requerido")]
        [MaxLength(50)]
        public string Estado { get; set; } = string.Empty;

        public string? Imagen { get; set; }

        [Required(ErrorMessage = "El tutor es requerido")]
        public Guid IdTutor { get; set; }

        [ForeignKey("IdTutor")]
        public virtual Tutor? Tutor { get; set; }
    }
}
