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
        Task<AiResponseDto> ProcessAsync(AiResponseDto dto);
        Task<AiResponseDto> ProcessSmartAsync(AiResponseDto dto);
    }
}
