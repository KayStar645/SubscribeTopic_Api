using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Faculty;
using Core.Application.DTOs.Student;
using Core.Application.Features.Students.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.Students.Handlers.Queries
{
    public class DetailStudentQueryHandler : IRequestHandler<DetailStudentRequest, Result<StudentDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailStudentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<StudentDto>> Handle(DetailStudentRequest request, CancellationToken cancellationToken)
        {
            var validator = new DetailBaseRequestValidator();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<StudentDto>.Failure(string.Join(", ", errorMessages), (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var query = _unitOfWork.Repository<Student>().GetByIdInclude(request.id);

                if (request.isAllDetail)
                {
                    query = _unitOfWork.Repository<Student>().AddInclude(query, x => x.Major);
                }
                else
                {
                    if (request.isGetMajor == true)
                    {
                        query = _unitOfWork.Repository<Student>().AddInclude(query, x => x.Major);
                    }
                }

                var findStudent = await query.SingleAsync();

                if (findStudent is null)
                {
                    return Result<StudentDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var studentDto = _mapper.Map<StudentDto>(findStudent);

                return Result<StudentDto>.Success(studentDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<StudentDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}