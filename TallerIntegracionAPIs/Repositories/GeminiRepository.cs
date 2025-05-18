using System.Text;
using Newtonsoft.Json;
using TallerIntegracionAPIs.Interfaces;
using Microsoft.Extensions.Configuration;
using TallerIntegracionAPIs.Interfaces;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TallerIntegracionAPIs.Data;
using TallerIntegracionAPIs.Models;

namespace TallerIntegracionAPIs.Repositories
{
    public class GeminiRepository : IChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ChatbotDbContext _dbContext;

        public GeminiRepository(IConfiguration configuration, ChatbotDbContext dbContext)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["Gemini:ApiKey"];
            _dbContext = dbContext;
        }

        public async Task<string> ObtenerRespuestaChatbot(string prompt)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

            var request = new
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

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
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
                    Proveedor = "Gemini",
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