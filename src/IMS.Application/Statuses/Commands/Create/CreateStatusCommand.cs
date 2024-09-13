using IMS.Application.Statuses;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Statuses.Commands.Create;

public sealed record CreateStatusCommand(string Name) : IRequest<Result<StatusDto>>;
