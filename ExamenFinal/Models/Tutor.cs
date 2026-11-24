using ExamenFinal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ExamenFinal.Models
{
    public class Tutor
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre del tutor es requerido")]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especialidad es requerida")]
        [MaxLength(100)]
        public string Especialidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "El formato de correo no es válido")]
        [MaxLength(150)]
        public string Correo { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual ICollection<Tesis> Tesis { get; set; } = new List<Tesis>();
    }
}
