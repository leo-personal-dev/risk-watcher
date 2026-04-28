using MediatR;
using Watcher.Domain.Commands.Response;

namespace Watcher.Domain.Commands.Request;

public class GetAllJobCategoriesCommand : IRequest<GetAllJobCategoriesResponse>
{
}