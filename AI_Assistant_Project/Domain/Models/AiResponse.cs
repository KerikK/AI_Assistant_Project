using AI_Assistant_Project.Models;

namespace Domain.Models
{
    public class AiResponse
    {
        public int Id { get; set; }
        public string Response { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = true;
        public int TokensUsed { get; set; }
        public int RequestId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public long ExecutionTimeMs { get; set; }
        public AiRequest Request { get; set; }
    }
}
