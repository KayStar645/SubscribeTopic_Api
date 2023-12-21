﻿using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Teacher;
using Core.Application.Features.Teachers.Requests.Queries;
using MediatR;
using AutoMapper;
using Core.Application.Transform;
using Core.Domain.Entities;
using System.Net;
using Core.Application.Responses;
using Microsoft.EntityFrameworkCore;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Faculty;

namespace Core.Application.Features.Teachers.Handlers.Queries
{
    public class DetailTeacherQueryHandler : IRequestHandler<DetailTeacherRequest, Result<TeacherDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailTeacherQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<TeacherDto>> Handle(DetailTeacherRequest request, CancellationToken cancellationToken)
        {
            var validator = new DetailBaseRequestValidator();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<TeacherDto>.Failure(string.Join(", ", errorMessages), (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var query = _unitOfWork.Repository<Teacher>().GetByIdInclude(request.id);

                if (request.isAllDetail || request.isGetDepartment == true)
                {
                    query = query.Include(x => x.Department).ThenInclude(x => x.HeadDepartment_Teacher);
                }

                var findTeacher = await query.SingleAsync();

                if (findTeacher is null)
                {
                    return Result<TeacherDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var teacherDto = _mapper.Map<TeacherDto>(findTeacher);

                return Result<TeacherDto>.Success(teacherDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<TeacherDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
