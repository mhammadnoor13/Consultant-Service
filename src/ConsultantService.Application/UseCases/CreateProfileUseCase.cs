using Application.Common.Persistence;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantService.Application.UseCases
{
    public class CreateProfileUseCase : ICreateProfileUseCase
    {
        private readonly IConsultantRepository _consultantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateProfileUseCase(IConsultantRepository consultantRepository, IUnitOfWork unitOfWork)
        {
            _consultantRepository = consultantRepository;
            _unitOfWork = unitOfWork;
        }



        public async Task <Guid> ExecuteAsync(Guid userId, string firstName, string lastName, string email, string speciality, CancellationToken ct)
        {

            var consultant = Consultant.Create(userId,firstName,lastName,email,speciality).Value;

            await _consultantRepository.AddAsync(consultant, ct);
            await _unitOfWork.CommitAsync(ct);
            return consultant.Id;


        }
    }
}
