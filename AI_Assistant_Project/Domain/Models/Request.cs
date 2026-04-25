using Domain.Identity;
using Domain.Models;

namespace AI_Assistant_Project.Models
{
    public class Request
    {
        public int Id { get; set; }
        public string Prompt { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty; //"OpenAI" "Gemini"
        public bool FromCache { get; set; } 
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public long ResponseTimeMs { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }

        public Response? Response { get; set; }
    }
}
