using Microsoft.AspNetCore.Mvc;
using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;

namespace Watcher.Api.Controllers;

[ApiController]
[Route("clusters")]
public class ClusterController : ControllerBase
{
    private readonly IClusterConfigurationService _clusterService;

    public ClusterController(IClusterConfigurationService clusterService)
    {
        _clusterService = clusterService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClusterConfiguration>>> GetAll()
    {
        var configurations = await _clusterService.GetAllAsync();
        return Ok(configurations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClusterConfiguration>> GetById(string id)
    {
        var configuration = await _clusterService.GetByIdAsync(id);
        if (configuration == null)
        {
            return NotFound(new { error = "Cluster not found" });
        }

        return Ok(configuration);
    }

    [HttpPost]
    public async Task<ActionResult<ClusterConfiguration>> Create([FromBody] ClusterConfiguration request)
    {
        var created = await _clusterService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ClusterConfiguration>> Update(string id, [FromBody] ClusterConfiguration request)
    {
        if (!string.Equals(id, request.Id, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { error = "Cluster ID in the path must match the request body." });
        }

        try
        {
            var updated = await _clusterService.UpdateAsync(request);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Cluster not found" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _clusterService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Cluster not found" });
        }
    }
}