using Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAIService
    {
        Task<AiResponseDto> AskGrokAsync(AiRequestDto requestDto);
        Task<AiResponseDto> AskGeminiAsync(AiRequestDto requestDto);
    }
}
