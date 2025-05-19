using System.Text;
using TallerIntegracionAPIs.Interfaces;
using Microsoft.Extensions.Configuration;
using TallerIntegracionAPIs.Data;
using Microsoft.EntityFrameworkCore;
using TallerIntegracionAPIs.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Net;

namespace TallerIntegracionAPIs.Repositories
{
    public class OpenAIRepository : IChatbotService
    {
        private readonly HttpClient _httpClient;

        public OpenAIRepository(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config["OpenAI:ApiKey"]}");
        }

        public async Task<string> ObtenerRespuestaChatbot(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return "El prompt no puede estar vacío.";

            try
            {
                var requestData = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                    new { role = "user", content = prompt }
                }
                };

                var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

                if (!response.IsSuccessStatusCode)
                    return $"Error de OpenAI: {response.StatusCode}";

                var responseContent = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var result = jsonDoc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return result?.Trim() ?? "La respuesta de OpenAI fue vacía.";
            }
            catch (Exception ex)
            {
                return $"Ocurrió un error al obtener la respuesta de OpenAI: {ex.Message}";
            }
        }
    }
}