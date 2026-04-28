using MediatR;
using Microsoft.AspNetCore.Mvc;
using Watcher.Domain.Commands.Request;
using Watcher.Domain.Commands.Response;

namespace Watcher.Api.Controllers;

[ApiController]
[Route("api/job-categories")]
public class JobCategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobCategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetAllJobCategoriesResponse>> GetAll()
    {
        var command = new GetAllJobCategoriesCommand();
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetJobCategoryResponse>> GetById(string id)
    {
        try
        {
            var command = new GetJobCategoryByIdCommand { JobCategoryId = id };
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<CreateJobCategoryResponse>> Create([FromBody] CreateJobCategoryCommand request)
    {
        var response = await _mediator.Send(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateJobCategoryResponse>> Update(string id, [FromBody] UpdateJobCategoryCommand request)
    {
        request.Id = id;
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var command = new DeleteJobCategoryCommand { JobCategoryId = id };
        await _mediator.Send(command);
        return NoContent();
    }
}