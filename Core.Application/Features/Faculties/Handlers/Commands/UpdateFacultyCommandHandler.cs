using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Faculty.Validators;
using Core.Application.DTOs.Faculty;
using Core.Application.Features.Faculties.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using System.Net;
using Core.Application.Services;

namespace Core.Application.Features.Faculties.Handlers.Commands
{
    internal class UpdateFacultyCommandHandler : IRequestHandler<UpdateFacultyRequest, Result<FacultyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateFacultyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<FacultyDto>> Handle(UpdateFacultyRequest request, CancellationToken cancellationToken)
        {
            var validator = new UpdateFacultyDtoValidator(_unitOfWork, request.updateFacultyDto.Id);
            var errorValidator = await validator.ValidateAsync(request.updateFacultyDto);

            if (errorValidator.IsValid == false)
            {
                var errorMessage = errorValidator.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<FacultyDto>.Failure(errorMessage, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var findFaculty = await _unitOfWork.Repository<Domain.Entities.Faculties>().GetByIdAsync(request.updateFacultyDto.Id);

                if (findFaculty is null)
                {
                    return Result<FacultyDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.updateFacultyDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                findFaculty.CopyPropertiesFrom(request.updateFacultyDto);

                var newFaculty = await _unitOfWork.Repository<Domain.Entities.Faculties>().UpdateAsync(findFaculty);
                await _unitOfWork.Save(cancellationToken);

                var facultyDto = _mapper.Map<FacultyDto>(newFaculty);

                return Result<FacultyDto>.Success(facultyDto, (int)HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return Result<FacultyDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }

            throw new NotImplementedException();
        }
    }
}
