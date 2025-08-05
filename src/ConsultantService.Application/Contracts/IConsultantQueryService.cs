using ConsultantService.Application.Dtos;
using Contracts.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantService.Application.Contracts
{
    public interface IConsultantQueryService
    {
        Task<IReadOnlyList<CaseToCardDto>> GetAssignedCasesAsync(Guid consultantId, CancellationToken ct);
    }
}
