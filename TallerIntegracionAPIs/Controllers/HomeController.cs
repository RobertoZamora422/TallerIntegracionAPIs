using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TallerIntegracionAPIs.Interfaces;
using TallerIntegracionAPIs.Models;
using TallerIntegracionAPIs.Repositories;

namespace TallerIntegracionAPIs.Controllers
{
    public class HomeController : Controller
    {
        private IChatbotService _chatbotService;

        public HomeController(IChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _chatbotService.ObtenerRespuestaChatbot("Resume en 5 palabras El QUijote de la Mancha");
            ViewBag.respuesta = response;
            return View();
        }
    }
}