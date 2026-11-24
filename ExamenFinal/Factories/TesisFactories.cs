using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using ExamenFinal.Interfaces;
using ExamenFinal.Models;
using ExamenFinal.Data;

namespace ExamenFinal.Factories
{
    public static class TesisFactories
    {
        public static void Seed(BaseDeDatos context)
        {
            // Si ya existen tesis registradas, no volvemos a sembrar para evitar duplicados
            if (context.tesis.Any()) return;

            // 1. Crear tutores si no existen (mínimo 4 para soportar 10 tesis sin exceder el límite de 3 por tutor)
            if (!context.tutores.Any())
            {
                var especialidades = new[]
                {
                    "Inteligencia Artificial y Deep Learning",
                    "Ciberseguridad y Auditoría de Sistemas",
                    "Ingeniería de Software y Arquitectura Cloud",
                    "Ciencia de Datos y Big Data",
                    "Redes y Sistemas Distribuidos"
                };

                var tutorFaker = new Faker<Tutor>("es")
                    .RuleFor(t => t.Id, f => Guid.NewGuid())
                    .RuleFor(t => t.Nombre, f => $"Ing. {f.Name.FullName()}")
                    .RuleFor(t => t.Especialidad, f => f.PickRandom(especialidades))
                    .RuleFor(t => t.Correo, (f, t) => f.Internet.Email(t.Nombre.Replace("Ing. ", "").Trim()));

                var tutores = tutorFaker.Generate(4);
                context.tutores.AddRange(tutores);
                context.SaveChanges();
            }

            var tutoresDisponibles = context.tutores.ToList();
            if (!tutoresDisponibles.Any()) return;


            var titulosAcademicos = new[]
            {
                "Implementación de Sistema de Reconocimiento Facial con Redes Convolucionales",
                "Optimización de Consultas y Particionado en Bases de Datos NoSQL Distribuidas",
                "Plataforma Web para Telemedicina con Microservicios en .NET y Docker",
                "Detección Temprana de Ciberamenazas en Redes Corporativas usando Machine Learning",
                "Diseño y Despliegue de Infraestructura Segura en la Nube mediante Terraform",
                "Aplicación Móvil IoT para Monitoreo Automatizado de Cultivos Hidropónicos",
                "Automatización de Pruebas de Calidad con Pipelines CI/CD en GitHub Actions",
                "Trazabilidad de Cadena de Suministro Farmacéutica basada en Smart Contracts y Blockchain",
                "Análisis de Sentimiento en Redes Sociales para Medición de Clima Organizacional",
                "Sistema Inteligente de Gestión Hospitalaria y Triaje Predictivo con Algoritmos Genéticos"
            };

            var estadosPosibles = new[] { "En Desarrollo", "Aprobado", "Revisión Final", "Defendida", "Pendiente" };
            var faker = new Faker("es");
            var listaTesis = new List<Tesis>();

            for (int i = 0; i < titulosAcademicos.Length; i++)
            {
                int indiceTutor = i / 3;
                if (indiceTutor >= tutoresDisponibles.Count)
                    indiceTutor = tutoresDisponibles.Count - 1;

                var tutorAsignado = tutoresDisponibles[indiceTutor];

                var tesis = new Tesis
                {
                    Id = Guid.NewGuid(),
                    Titulo = titulosAcademicos[i],
                    Estudiante = faker.Name.FullName(),
                    Estado = faker.PickRandom(estadosPosibles),
                    Imagen = $"/imagenes/tesis/tesis_{i + 1}.jpg",
                    IdTutor = tutorAsignado.Id
                };

                listaTesis.Add(tesis);
            }

            context.tesis.AddRange(listaTesis);
            context.SaveChanges();
        }
    }
}
