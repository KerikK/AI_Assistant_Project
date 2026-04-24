using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class AiResponseDto
    {
        public string Response { get; set; } = string.Empty;
        public string Provider { get; set; } = "Groq/Llama3";
        public bool FromCache { get; set; }
        public bool IsSuccess { get; set; } = true;
        public long ExecutionTimeMs { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
