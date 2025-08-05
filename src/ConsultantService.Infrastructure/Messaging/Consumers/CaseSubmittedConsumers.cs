using ConsultantService.Application.Consultants.Commands.AssignCase;
using ConsultantService.Application.Consultants.Commands.CreateConsultantFromEvent;
using Contracts;
using Contracts.Shared.Events;
using MassTransit;
using MediatR;
using Serilog;

namespace ConsultantService.Infrastructure.Messaging.Consumers
{
    public class CaseSubmittedConsumer : IConsumer<CaseSubmitted>
    {
        private readonly IMediator _mediator;
        private readonly IPublishEndpoint _publishEndpoint;

        public CaseSubmittedConsumer(IMediator mediator, IPublishEndpoint publishEndpoint)
        {
            _mediator = mediator;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<CaseSubmitted> context)
        {
            Log.Information("[CONSUME] got {CaseId} – {Spec}", context.Message.CaseId, context.Message.Speciality);

            var msg = context.Message;
            
            var consultantId = await _mediator.Send(new AssignCaseCommand(msg.CaseId, msg.Speciality), context.CancellationToken);

            await _publishEndpoint.Publish<CaseAssigned>(new
            {
                CaseId = msg.CaseId,
                ConsultantId = consultantId
            },
            context.CancellationToken);
        }
    }
}
