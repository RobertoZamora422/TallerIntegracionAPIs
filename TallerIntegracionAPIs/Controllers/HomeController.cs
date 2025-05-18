using Microsoft.AspNetCore.Mvc;
using TallerIntegracionAPIs.Data;
using TallerIntegracionAPIs.Models;
using TallerIntegracionAPIs.Repositories;

namespace TallerIntegracionAPIs.Controllers
{
    public class HomeController : Controller
    {
        private readonly ChatbotDbContext _context;
        private readonly GeminiRepository _gemini;
        private readonly OpenAIRepository _openai;

        public HomeController(ChatbotDbContext context, GeminiRepository gemini, OpenAIRepository openai)
        {
            _context = context;
            _gemini = gemini;
            _openai = openai;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var historial = _context.Respuestas
                .OrderByDescending(r => r.Fecha)
                .Take(10)
                .ToList();

            ViewBag.Historial = historial;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string prompt, string proveedor)
        {
            if (string.IsNullOrWhiteSpace(prompt) || string.IsNullOrWhiteSpace(proveedor))
            {
                ViewBag.Respuesta = "Por favor completa todos los campos.";
                ViewBag.Historial = _context.Respuestas.OrderByDescending(r => r.Fecha).Take(10).ToList();
                return View();
            }

            string respuesta = proveedor switch
            {
                "Gemini" => await _gemini.ObtenerRespuestaChatbot(prompt),
                "OpenAI" => await _openai.ObtenerRespuestaChatbot(prompt),
                _ => "Proveedor no válido."
            };

            var respuestaModel = new RespuestaAIModel
            {
                Prompt = prompt,
                Respuesta = respuesta,
                Proveedor = proveedor,
                GuardadoPor = "Zamora"
            };

            _context.Respuestas.Add(respuestaModel);
            await _context.SaveChangesAsync();

            ViewBag.Respuesta = respuesta;
            ViewBag.Historial = _context.Respuestas.OrderByDescending(r => r.Fecha).Take(10).ToList();

            return View();
        }
    }
}