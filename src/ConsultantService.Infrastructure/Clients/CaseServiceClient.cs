using ConsultantService.Application.Contracts;
using ConsultantService.Application.Dtos;
using Contracts.Shared.Responses;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsultantService.Infrastructure.Clients
{
    public class CaseServiceClient : ICaseServiceClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<CaseServiceClient> _logger;


        public CaseServiceClient(HttpClient http, ILogger<CaseServiceClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<IReadOnlyList<CaseToCardDto>> GetCasesByIdsAsync(IEnumerable<Guid> caseIds, CancellationToken ct)
        {

            try
            {
                _logger.LogInformation("Calling Case-Service bulk endpoint for {Count} IDs",
                                       caseIds.Count());

                var resp = await _http.PostAsJsonAsync("/cases/bulk", caseIds, ct);

                _logger.LogInformation("Received {StatusCode} from Case-Service",
                                       resp.StatusCode);

                var payload = await resp.Content.ReadAsStringAsync(ct);
                _logger.LogDebug("Response payload: {Payload}", payload);

                resp.EnsureSuccessStatusCode();

                var dtos = JsonSerializer.Deserialize<List<CaseToCardDto>>(payload,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                           ?? new List<CaseToCardDto>();

                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Case-Service bulk endpoint");
                throw;
            }
        }
    }
}
