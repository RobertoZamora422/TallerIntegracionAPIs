using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerIntegracionAPIs.Data;
using TallerIntegracionAPIs.Interfaces;
using TallerIntegracionAPIs.Models;
using TallerIntegracionAPIs.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TallerIntegracionAPIs.Controllers
{
    public class HomeController : Controller
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ChatbotDbContext _context;

        public HomeController(IServiceProvider serviceProvider, ChatbotDbContext context)
        {
            _serviceProvider = serviceProvider;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var historial = await _context.Respuestas
                .OrderByDescending(r => r.Fecha)
                .Take(10)
                .ToListAsync();

            ViewBag.Historial = historial;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string prompt, string proveedor, string guardadoPor)
        {
            if (string.IsNullOrWhiteSpace(prompt) || string.IsNullOrWhiteSpace(proveedor))
            {
                ModelState.AddModelError("", "Debes ingresar una pregunta y seleccionar un proveedor.");
                return await Index();
            }

            var chatbotService = ObtenerServicioPorProveedor(proveedor);
            if (chatbotService == null)
            {
                ModelState.AddModelError("", "Proveedor no válido.");
                return await Index();
            }

            string respuesta = await chatbotService.ObtenerRespuestaChatbot(prompt);
            await GuardarRespuestaBaseDatosLocal(prompt, respuesta, proveedor, guardadoPor);

            ViewBag.Respuesta = respuesta;
            ViewBag.ProveedorSeleccionado = proveedor;

            var historial = await _context.Respuestas
                .OrderByDescending(r => r.Fecha)
                .Take(10)
                .ToListAsync();

            ViewBag.Historial = historial;
            return View();
        }

        private IChatbotService ObtenerServicioPorProveedor(string proveedor)
        {
            return proveedor?.ToLower() switch
            {
                "openai" => (IChatbotService)_serviceProvider.GetService(typeof(OpenAIRepository)),
                "gemini" => (IChatbotService)_serviceProvider.GetService(typeof(GeminiRepository)),
                _ => null
            };
        }

        private async Task GuardarRespuestaBaseDatosLocal(string prompt, string respuesta, string proveedor, string guardadoPor)
        {
            var registro = new RespuestaAIModel
            {
                Prompt = prompt,
                Respuesta = respuesta,
                Fecha = DateTime.Now,
                Proveedor = proveedor,
                GuardadoPor = guardadoPor ?? "Anónimo"
            };

            _context.Respuestas.Add(registro);
            await _context.SaveChangesAsync();
        }
    }
}