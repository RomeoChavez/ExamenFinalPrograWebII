using System;
using System.Collections.Generic;
using Contrato.DTO;
using ExamenFinal.Models;

namespace ExamenFinal.Interfaces
{
    public interface ITutor
    {
        IEnumerable<Tutor> ObtenerTodos();
        Tutor? ObtenerPorId(Guid id);
        Tutor Crear(TutorDTO dto);
        bool ExisteCorreo(string correo);
        int ContarTesis(Guid idTutor);
    }
}
