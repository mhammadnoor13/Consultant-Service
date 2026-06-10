using ConsultantService.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantService.Application.Contracts
{
    public interface ICaseServiceClient
    {
        Task<IReadOnlyList<CaseToCardDto>> GetCasesByIdsAsync(IEnumerable<Guid> caseIds, CancellationToken ct);

    }
}
