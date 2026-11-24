using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Contrato.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProjecBlazor.Pages.Tutores
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _http;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("Api");
        }

        [BindProperty]
        public TutorDTO Tutor { get; set; } = new();

        public string? MensajeError { get; set; }

        public void OnGet()
        {
        }

        // 1) POST TUTOR
        // - NO PUEDE DUPLICARSE TUTOR
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var respuesta = await _http.PostAsJsonAsync("api/tutor", Tutor);

                if (respuesta.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = $"El tutor '{Tutor.Nombre}' fue registrado exitosamente.";
                    return RedirectToPage("Index");
                }

                var cuerpo = await respuesta.Content.ReadAsStringAsync();
                try
                {
                    using var doc = JsonDocument.Parse(cuerpo);
                    if (doc.RootElement.TryGetProperty("mensaje", out var msj))
                    {
                        MensajeError = msj.GetString();
                    }
                    else
                    {
                        MensajeError = "No se pudo registrar el tutor.";
                    }
                }
                catch
                {
                    MensajeError = string.IsNullOrWhiteSpace(cuerpo) ? "Error al registrar el tutor." : cuerpo;
                }
            }
            catch (HttpRequestException)
            {
                MensajeError = "No se pudo conectar con la API.";
            }
            catch (Exception ex)
            {
                MensajeError = $"Error inesperado: {ex.Message}";
            }

            return Page();
        }
    }
}
