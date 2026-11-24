using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Contrato.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cliente.Pages.Tesis
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _http;
        public string ApiBaseUrl { get; }

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("Api");
            ApiBaseUrl = _http.BaseAddress?.ToString().TrimEnd('/') ?? "";
        }

        public List<TesisDTO> TesisList { get; private set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? TerminoBusqueda { get; set; }

        public string? MensajeError { get; private set; }

        public async Task OnGetAsync()
        {
            try
            {
                string url = string.IsNullOrWhiteSpace(TerminoBusqueda)
                    ? "api/tesis"
                    : $"api/tesis/buscar?termino={Uri.EscapeDataString(TerminoBusqueda)}";

                TesisList = await _http.GetFromJsonAsync<List<TesisDTO>>(url) ?? new List<TesisDTO>();
            }
            catch (HttpRequestException)
            {
                MensajeError = "No se pudo conectar con la API de Tesis. Asegúrese de que el backend esté en ejecución.";
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar la información: {ex.Message}";
            }
        }
    }
}
