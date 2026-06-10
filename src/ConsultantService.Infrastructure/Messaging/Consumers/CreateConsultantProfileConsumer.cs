using MassTransit;
using ConsultantService.Application.UseCases;
using ConsultantPlatform.Contracts.Commands;
using ConsultantPlatform.Contracts.Responses;

namespace ConsultantService.Api.Consumers
{
    public class CreateConsultantProfileConsumer : IConsumer<CreateConsultantProfileCommand>
    {
        private readonly ICreateProfileUseCase _createProfileUseCase;

        public CreateConsultantProfileConsumer(ICreateProfileUseCase createProfileUseCase)
        {
            _createProfileUseCase = createProfileUseCase;
        }


        public async Task Consume(ConsumeContext<CreateConsultantProfileCommand> context)
        {
            try
            {
                var cmd = context.Message;
                var userId = await _createProfileUseCase.ExecuteAsync(
                    cmd.UserId,
                    cmd.FirstName,
                    cmd.LastName,
                    cmd.Email,
                    cmd.Speciality,
                    context.CancellationToken);

                await context.RespondAsync(new ConsultantProfileCreatedResponse(userId));
            }
            catch (InvalidOperationException ex)    
            {
                // MassTransit will automatically publish a Fault<CreateUserCommand>
                throw;
            }
        }


    }
}
