using System.Text;
using Newtonsoft.Json;
using TallerIntegracionAPIs.Interfaces;
using Microsoft.Extensions.Configuration;
using TallerIntegracionAPIs.Data;
using Microsoft.EntityFrameworkCore;
using TallerIntegracionAPIs.Models;

namespace TallerIntegracionAPIs.Repositories
{
    public class OpenAIRepository : IChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ChatbotDbContext _dbContext;

        public OpenAIRepository(IConfiguration configuration, ChatbotDbContext dbContext)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["OpenAI:ApiKey"];
            _dbContext = dbContext;
        }

        public async Task<string> ObtenerRespuestaChatbot(string prompt)
        {
            var url = "https://api.openai.com/v1/chat/completions";

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

        public bool GuardarRespuestaBaseDatosLocal(string prompt, string respuesta)
        {
            try
            {
                var nuevaRespuesta = new RespuestaAIModel
                {
                    Prompt = prompt,
                    Respuesta = respuesta,
                    Fecha = DateTime.Now,
                    Proveedor = "OpenAI",
                    GuardadoPor = "Zamora"
                };

                _dbContext.Respuestas.Add(nuevaRespuesta);
                _dbContext.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
