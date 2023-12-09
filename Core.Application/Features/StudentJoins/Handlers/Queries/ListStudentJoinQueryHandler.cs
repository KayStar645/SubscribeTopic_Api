using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.StudentJoin;
using Core.Application.Features.StudentJoins.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.StudentJoins.Handlers.Queries
{
    public class ListStudentJoinQueryHandler : IRequestHandler<ListStudentJoinRequest, PaginatedResult<List<StudentJoinDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        public ListStudentJoinQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<PaginatedResult<List<StudentJoinDto>>> Handle(ListStudentJoinRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListBaseRequestValidator<StudentJoinDto>();
            var result = validator.Validate(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<StudentJoinDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<StudentJoin>().GetAllInclude();

            if (request.periodId != null)
            {
                query = query.Where(x => x.RegistrationPeriodId == request.periodId);
            }

            if(request.majorId != null)
            {
                query = query.Where(x => x.Student.MajorId == request.majorId);
            }    
            else if(request.industryId != null)
            {
                query = query.Where(x => x.Student.Major.IndustryId == request.industryId);
            }   
            else
            {
                query = query.Where(x => x.Student.Major.Industry.FacultyId == request.facultyId);
            }

            if (request.isAllDetail)
            {
                query = _unitOfWork.Repository<StudentJoin>().AddInclude(query, x => x.Student);
                query = _unitOfWork.Repository<StudentJoin>().AddInclude(query, x => x.RegistrationPeriod);
            }
            else
            {
                if (request.isGetStudent)
                {
                    query = _unitOfWork.Repository<StudentJoin>().AddInclude(query, x => x.Student);
                }

                if (request.isGetRegistrationPeriod)
                {
                    query = _unitOfWork.Repository<StudentJoin>().AddInclude(query, x => x.RegistrationPeriod);
                }
            }

            query = _sieveProcessor.Apply(sieve, query);

            int totalCount = await query.CountAsync();

            var studentJoins = await query.ToListAsync();

            var mapStudentJoins = _mapper.Map<List<StudentJoinDto>>(studentJoins);

            return PaginatedResult<List<StudentJoinDto>>.Success(
                mapStudentJoins, totalCount, request.page,
                request.pageSize);
        }
    }
}
