using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Student;
using Core.Application.DTOs.Teacher;
using Core.Application.Features.Teachers.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Teachers.Handlers.Queries
{
    public class ListTeacherRequestHandler : IRequestHandler<ListTeacherRequest, PaginatedResult<List<TeacherDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;

        public ListTeacherRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }
        public async Task<PaginatedResult<List<TeacherDto>>> Handle(ListTeacherRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListTeacherDtoValidator(_unitOfWork, request.departmentId);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<TeacherDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Teacher>().GetAllInclude();

            if (request.departmentId != null) 
            {
                query = query.Where(x => x.DepartmentId == request.departmentId);
            }    
            else
            {
                query = query.Where(x => x.Department.FacultyId == request.facultyId);
            }

            if(string.IsNullOrEmpty(request.type) == false)
            {
                query = query.Where(x => x.Type == request.type);
            }    

            if (request.isAllDetail)
            {
                query = _unitOfWork.Repository<Teacher>().AddInclude(query, x => x.Department);
            }
            else
            {
                if (request.isGetDepartment == true)
                {
                    query = _unitOfWork.Repository<Teacher>().AddInclude(query, x => x.Department);
                }
            }
            
            int totalCount = await query.CountAsync();

            query = _sieveProcessor.Apply(sieve, query);

            var teachers = await query.ToListAsync();

            var mapTeachers = _mapper.Map<List<TeacherDto>>(teachers);
            return PaginatedResult<List<TeacherDto>>.Success(
                mapTeachers, totalCount, request.page,
                request.pageSize);
        }
    }
}
