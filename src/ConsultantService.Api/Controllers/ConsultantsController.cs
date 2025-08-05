using Application.Consultants.Commands.CreateConsultant;
using Application.Consultants.DTOs;
using Application.Consultants.Queries;
using Application.Consultants.Queries.GetAll;
using Application.Consultants.Queries.GetById;
using ConsultantService.Application.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{

    [ApiController]
    [Route("consultants")]
    public class ConsultantsController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly IConsultantQueryService _consultantQueryService;

        public ConsultantsController(IMediator mediator, IConsultantQueryService consultantQueryService)
        {
            _mediator = mediator;
            _consultantQueryService = consultantQueryService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateConsultantDto dto)
        {
            var cmd = new CreateConsultantCmd(dto.FirstName, dto.LastName, dto.Email, dto.Speciality);
            var result = await _mediator.Send(cmd);

            return result.IsSuccess
                ? CreatedAtRoute(
                    routeName: "GetConsultantById",
                    routeValues: new { id = result.Value },
                    value: result.Value)
                : BadRequest(result.Error);
                ;

        }

        [HttpGet("{id:Guid}",Name ="GetConsultantById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetConsultantByIdQuery(id));

            return result.IsSuccess
                ? Ok(result.Value)
                : NotFound(result.Error);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllConsultantsQuery());

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }


        [HttpGet("assigned-cases")]
        public async Task<IActionResult> GetAssignedCases( CancellationToken ct)
        {
            if (!Guid.TryParse(Request.Headers["X-User-Id"], out var consultantId))
                return Unauthorized();

            var cases = await _consultantQueryService.GetAssignedCasesAsync(consultantId, ct);

            return Ok(cases);
        }

        [HttpGet("{consultantId}/assigned-cases")]
        public async Task<IActionResult> GetAssignedCases(Guid consultantId, CancellationToken ct)
        {
            var cases =await _consultantQueryService.GetAssignedCasesAsync(consultantId,ct);

            return Ok(cases);
        }




    }
}
