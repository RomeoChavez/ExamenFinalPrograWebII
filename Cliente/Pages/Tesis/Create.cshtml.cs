using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Contrato.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProjecBlazor.Pages.Tesis
{
    public class TutorOpcionViewModel
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public int TotalTesis { get; set; }
    }

    public class CreateModel : PageModel
    {
        private readonly HttpClient _http;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("Api");
        }

        [BindProperty]
        public TesisDTO Tesis { get; set; } = new();

        [BindProperty]
        public IFormFile? ArchivoImagen { get; set; }

        public List<TutorOpcionViewModel> Tutores { get; set; } = new();

        public string? MensajeError { get; set; }

        public async Task OnGetAsync()
        {
            await CargarTutoresAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CargarTutoresAsync();
                return Page();
            }

            try
            {
                using var formContent = new MultipartFormDataContent();
                formContent.Add(new StringContent(Tesis.Titulo ?? string.Empty), "Titulo");
                formContent.Add(new StringContent(Tesis.Estudiante ?? string.Empty), "Estudiante");
                formContent.Add(new StringContent(Tesis.Estado ?? string.Empty), "Estado");
                formContent.Add(new StringContent(Tesis.IdTutor.ToString()), "IdTutor");

                if (ArchivoImagen != null && ArchivoImagen.Length > 0)
                {
                    var streamContent = new StreamContent(ArchivoImagen.OpenReadStream());
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(ArchivoImagen.ContentType);
                    formContent.Add(streamContent, "archivoImagen", ArchivoImagen.FileName);
                }

                var respuesta = await _http.PostAsync("api/tesis", formContent);

                if (respuesta.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "La tesis fue registrada exitosamente con su portada y tutor.";
                    return RedirectToPage("Index");
                }

                // Capturar mensaje de error específico del backend (por ej. tutor con 3 tesis o tesis duplicada)
                var cuerpoError = await respuesta.Content.ReadAsStringAsync();
                try
                {
                    using var doc = JsonDocument.Parse(cuerpoError);
                    if (doc.RootElement.TryGetProperty("mensaje", out var msjElement))
                    {
                        MensajeError = msjElement.GetString();
                    }
                    else
                    {
                        MensajeError = "Error al registrar la tesis en el servidor.";
                    }
                }
                catch
                {
                    MensajeError = string.IsNullOrWhiteSpace(cuerpoError) ? "Error al registrar la tesis." : cuerpoError;
                }
            }
            catch (HttpRequestException)
            {
                MensajeError = "No se pudo conectar con la API. Verifique que el servicio esté corriendo.";
            }
            catch (Exception ex)
            {
                MensajeError = $"Error inesperado: {ex.Message}";
            }

            await CargarTutoresAsync();
            return Page();
        }

        private async Task CargarTutoresAsync()
        {
            try
            {
                Tutores = await _http.GetFromJsonAsync<List<TutorOpcionViewModel>>("api/tutor") ?? new();
            }
            catch
            {
                Tutores = new();
            }
        }
    }
}
