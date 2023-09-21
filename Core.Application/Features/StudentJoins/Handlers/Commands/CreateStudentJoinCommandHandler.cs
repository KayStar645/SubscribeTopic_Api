using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.StudentJoin;
using Core.Application.DTOs.StudentJoin.Validators;
using Core.Application.Features.StudentJoins.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.StudentJoins.Handlers.Commands
{
    public class CreateStudentJoinCommandHandler : IRequestHandler<CreateStudentJoinRequest, Result<StudentJoinDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateStudentJoinCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<StudentJoinDto>> Handle(CreateStudentJoinRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateStudentJoinDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.createStudentJoinDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<StudentJoinDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var studentJoin = _mapper.Map<StudentJoin>(request.createStudentJoinDto);

                var newStudentJoin = await _unitOfWork.Repository<StudentJoin>().AddAsync(studentJoin);
                await _unitOfWork.Save(cancellationToken);

                var StudentJoinDto = _mapper.Map<StudentJoinDto>(newStudentJoin);

                return Result<StudentJoinDto>.Success(StudentJoinDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Result<StudentJoinDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
