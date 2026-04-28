using MediatR;
using Microsoft.AspNetCore.Mvc;
using Watcher.Api.Services;
using Watcher.Domain.Commands.Request;
using Watcher.Domain.Commands.Response;

namespace Watcher.Api.Controllers;

[ApiController]
[Route("customers")]
public class CustomerController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly Services.RequestMetrics _metrics;

    public CustomerController(IMediator mediator, Services.RequestMetrics metrics)
    {
        _mediator = mediator;
        _metrics = metrics;
    }

    [HttpPost("classify")]
    public async Task<ActionResult<ClassifyCustomerResponse>> Classify([FromBody] ClassifyCustomerCommand request)
    {
        var response = await _mediator.Send(request);
        _metrics.ClassificationRequests++;
        return Ok(response);
    }
}