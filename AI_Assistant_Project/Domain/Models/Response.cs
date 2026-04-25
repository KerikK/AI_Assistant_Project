using AI_Assistant_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Response
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public int TokensUsed { get; set; }
        public long ExecutionTimeMs { get; set; }
        public DateTime CreatedAt { get; set; }

        public int RequestId { get; set; }
        public Request? Request { get; set; }
    }
}
