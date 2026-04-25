using Domain.Identity;
using Domain.Models;
using System.Security.Cryptography.X509Certificates;

namespace AI_Assistant_Project.Models
{
    public class AiRequest
    {
        public int Id { get; set; }
        public string Prompt { get; set; } = string.Empty;
        public int ResponseId { get; set; }
        public AiResponse Response { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
