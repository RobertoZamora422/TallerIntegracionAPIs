using Microsoft.AspNetCore.Mvc;
using TallerIntegracionAPIs.Repositories;

namespace TallerIntegracionAPIs.Controllers
{
    public class HomeController : Controller
    {
        private readonly GeminiRepository _gemini;
        private readonly OpenAIRepository _openai;

        public HomeController(GeminiRepository gemini, OpenAIRepository openai)
        {
            _gemini = gemini;
            _openai = openai;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string prompt, string proveedor)
        {
            if (string.IsNullOrWhiteSpace(prompt) || string.IsNullOrWhiteSpace(proveedor))
            {
                ViewBag.Respuesta = "Por favor completa todos los campos.";
                return View();
            }

            string respuesta = proveedor switch
            {
                "Gemini" => await _gemini.ObtenerRespuestaChatbot(prompt),
                "OpenAI" => await _openai.ObtenerRespuestaChatbot(prompt),
                _ => "Proveedor no válido."
            };

            ViewBag.Respuesta = respuesta;
            return View();
        }
    }
}
