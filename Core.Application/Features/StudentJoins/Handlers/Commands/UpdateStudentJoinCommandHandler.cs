using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.StudentJoin;
using Core.Application.DTOs.StudentJoin.Validators;
using Core.Application.Features.StudentJoins.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.StudentJoins.Handlers.Commands
{
    public class UpdateStudentJoinCommandHandler : IRequestHandler<UpdateStudentJoinRequest, Result<StudentJoinDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateStudentJoinCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<StudentJoinDto>> Handle(UpdateStudentJoinRequest request, CancellationToken cancellationToken)
        {
            var validator = new UpdateStudentJoinDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.updateStudentJoinDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<StudentJoinDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var findStudentJoin = await _unitOfWork.Repository<StudentJoin>().GetByIdAsync(request.updateStudentJoinDto.Id);

                if (findStudentJoin is null)
                {
                    return Result<StudentJoinDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.updateStudentJoinDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                findStudentJoin.CopyPropertiesFrom(request.updateStudentJoinDto);

                var newStudentJoin = await _unitOfWork.Repository<StudentJoin>().UpdateAsync(findStudentJoin);
                await _unitOfWork.Save(cancellationToken);

                var studentJoinDto = _mapper.Map<StudentJoinDto>(newStudentJoin);

                return Result<StudentJoinDto>.Success(studentJoinDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<StudentJoinDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
