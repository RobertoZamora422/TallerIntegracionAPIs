using System.Text;
using TallerIntegracionAPIs.Interfaces;
using Microsoft.Extensions.Configuration;
using TallerIntegracionAPIs.Interfaces;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TallerIntegracionAPIs.Data;
using TallerIntegracionAPIs.Models;
using System.Text.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TallerIntegracionAPIs.Repositories
{
    public class GeminiRepository : IChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiRepository(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["Gemini:ApiKey"]!;
        }

        public async Task<string> ObtenerRespuestaChatbot(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return "El prompt no puede estar vacío.";

            try
            {
                var requestData = new
                {
                    contents = new[]
                    {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
                };

                var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}", content);

                if (!response.IsSuccessStatusCode)
                    return $"Error de Gemini: {response.StatusCode}";

                var responseContent = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var result = jsonDoc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return result?.Trim() ?? "La respuesta de Gemini fue vacía.";
            }
            catch (Exception ex)
            {
                return $"Ocurrió un error al obtener la respuesta de Gemini: {ex.Message}";
            }
        }
    }
}