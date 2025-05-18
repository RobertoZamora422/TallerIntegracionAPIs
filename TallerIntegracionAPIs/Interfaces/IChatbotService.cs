namespace TallerIntegracionGemini.Interfaces
{
    public interface IChatbotService
    {
        public Task<string> ObtenerRespuestaChatbot(string prompt);
        public bool GuardarRespuestaBaseDatosLocal(string prompt, string respuesta);
    }
}