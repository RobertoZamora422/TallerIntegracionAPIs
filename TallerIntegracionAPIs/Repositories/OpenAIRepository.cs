using TallerIntegracionAPIs.Interfaces;

namespace TallerIntegracionAPIs.Repositories
{
    public class OpenAIRepository : IChatbotService
    {
        public bool GuardarRespuestaBaseDatosLocal(string prompt, string respuesta)
        {
            throw new NotImplementedException();
        }

        public Task<string> ObtenerRespuestaChatbot(string prompt)
        {
            throw new NotImplementedException();
        }
    }
}
