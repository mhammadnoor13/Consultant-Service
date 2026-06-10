using ConsultantService.Application.Consultants.Commands.AssignCase;
using MassTransit;
using MediatR;
using Serilog;
using ConsultantPlatform.Contracts.Events;

namespace ConsultantService.Infrastructure.Messaging.Consumers
{
    public class CaseSubmittedConsumer : IConsumer<CaseSubmittedEvent>
    {
        private readonly IMediator _mediator;
        private readonly IPublishEndpoint _publishEndpoint;

        public CaseSubmittedConsumer(IMediator mediator, IPublishEndpoint publishEndpoint)
        {
            _mediator = mediator;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<CaseSubmittedEvent> context)
        {
            Log.Information("[CONSUME] got {CaseId} – {Spec}", context.Message.CaseId, context.Message.Speciality);

            var msg = context.Message;
            
            var consultantId = await _mediator.Send(new AssignCaseCommand(msg.CaseId, msg.Speciality), context.CancellationToken);

            await _publishEndpoint.Publish<CaseAssignedEvent>(new
            {
                CaseId = msg.CaseId,
                ConsultantId = consultantId
            },
            context.CancellationToken);
        }
    }
}
