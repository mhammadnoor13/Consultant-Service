using MassTransit;
using MediatR;
using ConsultantService.Application.Consultants.Commands.CreateConsultantFromEvent; 
using System.Text.Json;
using Serilog;
using ConsultantPlatform.Contracts.Events;

public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly IMediator _mediator;
    public UserRegisteredConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var msg = context.Message;
        var command = new CreateConsultantFromEventCommand(
            msg.Id,
            msg.Email,
            msg.FirstName,
            msg.LastName,
            msg.Speciality
        );

        await _mediator.Send(command);
    }
}
