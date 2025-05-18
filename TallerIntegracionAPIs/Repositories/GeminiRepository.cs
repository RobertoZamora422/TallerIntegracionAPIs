using TallerIntegracionAPIs.Interfaces;

namespace TallerIntegracionAPIs.Repositories
{
    public class GeminiRepository : IChatbotService
    {
        public Task<string> ObtenerRespuestaChatbot(string prompt)
        {
            throw new NotImplementedException();
        }

        public bool GuardarRespuestaBaseDatosLocal(string prompt, string respuesta)
        {
            throw new NotImplementedException();
        }
    }
}
