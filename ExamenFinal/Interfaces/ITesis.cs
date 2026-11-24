using System;
using System.Collections.Generic;
using Contrato.DTO;
using ExamenFinal.Models;

namespace ExamenFinal.Interfaces
{
    public interface ITesis
    {
        IEnumerable<Tesis> ObtenerTodas();
        Tesis? ObtenerPorId(Guid id);
        IEnumerable<Tesis> Buscar(string? termino);
        Tesis Crear(TesisDTO dto);
        bool ExisteTitulo(string titulo);
        int ContarPorTutor(Guid idTutor);
    }
}
