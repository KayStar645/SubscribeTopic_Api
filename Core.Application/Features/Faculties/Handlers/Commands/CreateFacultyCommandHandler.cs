using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Faculty;
using Core.Application.DTOs.Faculty.Validators;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.Faculties.Handlers.Commands
{
    public class CreateFacultyCommandHandler : IRequestHandler<Requests.Commands.CreateFacultyRequest, Result<FacultyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateFacultyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<FacultyDto>> Handle(Requests.Commands.CreateFacultyRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateFacultyDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.createFacultyDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<FacultyDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var faculty = _mapper.Map<Domain.Entities.Faculties>(request.createFacultyDto);

                var newFaculty = await _unitOfWork.Repository<Domain.Entities.Faculties>().AddAsync(faculty);
                await _unitOfWork.Save(cancellationToken);

                var facultyDto = _mapper.Map<FacultyDto>(newFaculty);

                return Result<FacultyDto>.Success(facultyDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Result<FacultyDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
