using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.DepartmentDuty.Validators;
using Core.Application.DTOs.DepartmentDuty;
using Core.Application.Features.DepartmentDuties.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.DepartmentDuties.Handlers.Commands
{
    public class CreateDepartmentDutyCommandHandler : IRequestHandler<CreateDepartmentDutyRequest, Result<DepartmentDutyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateDepartmentDutyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DepartmentDutyDto>> Handle(CreateDepartmentDutyRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateDepartmentDutyDtoValidator(_unitOfWork,
                request.CreateDepartmentDutyDto.TimeStart ?? DateTime.Now);
            var validatorResult = await validator.ValidateAsync(request.CreateDepartmentDutyDto);

            if (validatorResult.IsValid == false)
            {
                var errorMessages = validatorResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<DepartmentDutyDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var DepartmentDuty = _mapper.Map<DepartmentDuty>(request.CreateDepartmentDutyDto);

                var newDepartmentDuty = await _unitOfWork.Repository<DepartmentDuty>().AddAsync(DepartmentDuty);
                await _unitOfWork.Save(cancellationToken);

                var DepartmentDutyDto = _mapper.Map<DepartmentDutyDto>(newDepartmentDuty);

                return Result<DepartmentDutyDto>.Success(DepartmentDutyDto, (int)HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return Result<DepartmentDutyDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
        }
    }
}
