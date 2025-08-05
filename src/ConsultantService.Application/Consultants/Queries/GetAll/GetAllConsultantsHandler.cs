using Application.Common.Persistence;
using Domain.Entities;
using Domain.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Consultants.Queries.GetAll;

public sealed class GetAllConsultantsHandler
    : IRequestHandler<GetAllConsultantsQuery, Result<IEnumerable<Consultant>>>
{
    private readonly IConsultantRepository _repo;

    public GetAllConsultantsHandler(IConsultantRepository repo)
        => _repo = repo;

    public async Task<Result<IEnumerable<Consultant>>> Handle(
        GetAllConsultantsQuery request,
        CancellationToken ct)
    {
        return await _repo.GetAllAsync(ct);
    }
}