using MediatR;
using Watcher.Domain.Commands.Response;

namespace Watcher.Domain.Commands.Request;

public class UpdateJobCategoryCommand : IRequest<UpdateJobCategoryResponse>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Multiplier { get; set; }
    public List<string> Keywords { get; set; } = new();
}