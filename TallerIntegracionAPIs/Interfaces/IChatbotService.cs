namespace TallerIntegracionAPIs.Interfaces
{
    public interface IChatbotService
    {
        Task<string> ObtenerRespuestaChatbot(string prompt);
    }
}