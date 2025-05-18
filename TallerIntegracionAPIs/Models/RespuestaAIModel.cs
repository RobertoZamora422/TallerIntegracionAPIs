using System;
using System.ComponentModel.DataAnnotations;

namespace TallerIntegracionAPIs.Models
{
    public class RespuestaAIModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Prompt { get; set; }

        [Required]
        public string Respuesta { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        public string Proveedor { get; set; }

        public string GuardadoPor { get; set; }
    }
}