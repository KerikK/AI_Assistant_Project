namespace Domain.DTO
{
    public class AiResponseDto
    {
        public string Response { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public bool FromCache { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public long ExecutionTimeMs { get; set; }
    }
}
