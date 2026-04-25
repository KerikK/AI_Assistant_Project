using AI_Assistant_Project.Models;
using Domain.Identity;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAIService
    {
        Task<AiResponse> AskGrokAsync(AiRequest req);
        Task<AiResponse> AskGeminiAsync(AiRequest res);
    }
}
