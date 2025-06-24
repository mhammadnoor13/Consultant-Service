using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantService.Application.UseCases
{
    public interface ICreateProfileUseCase
    {
        Task <Guid> ExecuteAsync(Guid userId, string firstName, string lastName, string email, string speciality, CancellationToken ct);

    }
}
