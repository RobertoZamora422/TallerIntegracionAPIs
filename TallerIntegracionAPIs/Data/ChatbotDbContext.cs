using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TallerIntegracionAPIs.Models;

namespace TallerIntegracionAPIs.Data
{
    public class ChatbotDbContext : DbContext
    {
        public ChatbotDbContext(DbContextOptions<ChatbotDbContext> options)
            : base(options)
        {
        }

        public DbSet<RespuestaAIModel> Respuestas { get; set; }
    }
}