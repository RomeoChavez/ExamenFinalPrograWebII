using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProjecBlazor.Pages.Tutores
{
    public class TutorItemViewModel
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public int TotalTesis { get; set; }
    }

    public class IndexModel : PageModel
    {
        private readonly HttpClient _http;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("Api");
        }

        public List<TutorItemViewModel> Tutores { get; private set; } = new();
        public string? MensajeError { get; private set; }

        public async Task OnGetAsync()
        {
            try
            {
                Tutores = await _http.GetFromJsonAsync<List<TutorItemViewModel>>("api/tutor")
                    ?? new List<TutorItemViewModel>();
            }
            catch (HttpRequestException)
            {
                MensajeError = "No se pudo conectar con la API de Tutores.";
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar tutores: {ex.Message}";
            }
        }
    }
}
