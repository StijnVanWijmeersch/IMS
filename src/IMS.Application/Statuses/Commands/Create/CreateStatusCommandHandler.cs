using IMS.Application.Abstractions;
using IMS.Application.Mappers;
using IMS.Application.Statuses.Contracts;
using IMS.Domain.Orders;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Statuses.Commands.Create;

internal sealed class CreateStatusCommandHandler : IRequestHandler<CreateStatusCommand, Result<StatusDto>>
{
    private readonly IStatusRepository _statusRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStatusCommandHandler(
        IStatusRepository statusRepository,
        IUnitOfWork unitOfWork)
    {
        _statusRepository = statusRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<StatusDto>> Handle(CreateStatusCommand request, CancellationToken cancellationToken)
    {
        var name = new Name(request.Name);

        var existingStatus = await _statusRepository.ExistsByNameAsync(name, cancellationToken);

        if (existingStatus)
        {
            return Result.Failure<StatusDto>(StatusErrors.StatusAlreadyExists(name));
        }

        try
        {
            var newStatus = Status.Create(request.Name);

            _statusRepository.Add(newStatus);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(newStatus.ToStatusDto());
        }

        catch (Exception ex)
        {
            return Result.Failure<StatusDto>(StatusErrors.CreateStatusFailed(request.Name, ex.Message));
        }
    }
}
