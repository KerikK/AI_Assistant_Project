using AI_Assistant_Project.Models;
using BLL.Interfaces;
using Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AIService : IAIService
    {
        private readonly IService<Request> _requestService;
        public AIService(IService<Request> requestService)
        {
            _requestService = requestService;
        }

        public Task<AiResponseDto> ProcessAsync(AiResponseDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<AiResponseDto> ProcessSmartAsync(AiResponseDto dto)
        {
            throw new NotImplementedException();
        }

        private async Task<AiResponseDto> SendRequest(AiResponseDto dto)
        {

            throw new NotImplementedException();

        }
    }
}
