using Application.Common.Persistence;
using ConsultantService.Application.Contracts;
using ConsultantService.Application.Dtos;
using Contracts.Shared.Responses;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantService.Application.Services
{
    public class ConsultantQueryService : IConsultantQueryService
    {
        private readonly ICaseServiceClient _caseServiceClient;
        private readonly IConsultantRepository _consultantRepository;

        public ConsultantQueryService(IConsultantRepository consultantRepository, ICaseServiceClient caseServiceClient)
        {
            _consultantRepository = consultantRepository;
            _caseServiceClient = caseServiceClient;
        }

        public async Task<IReadOnlyList<CaseToCardDto>> GetAssignedCasesAsync(Guid consultantId, CancellationToken ct)
        {
            var result = await _consultantRepository.GetAssingedCases(consultantId);

            if (!result.IsSuccess)
                throw new KeyNotFoundException(result.Error);

            var caseIds = result.Value;

            if (caseIds == null || !caseIds.Any())
                return Array.Empty<CaseToCardDto>();

            var cases = await _caseServiceClient.GetCasesByIdsAsync(caseIds, ct);
            return cases;
        }
    }
}
