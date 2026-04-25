using AI_Assistant_Project.Models;
using Domain.DTO;
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
        Task<Response> AskGroqAsync(Request request);
        Task<Response> AskGeminiAsync(Request request);
    }
}
