using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Major;
using Core.Application.DTOs.StudentJoin;
using Core.Application.DTOs.Teacher;
using Core.Application.DTOs.ThesisRegistration;
using Core.Application.Features.ThesisRegistrations.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.ThesisRegistrationRegistrations.Handlers.Queries
{
    public class ListThesisRegistrationRequestHandler : IRequestHandler<ListThesisRegistrationRequest, PaginatedResult<List<ThesisRegistrationDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;

        public ListThesisRegistrationRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<PaginatedResult<List<ThesisRegistrationDto>>> Handle(ListThesisRegistrationRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListThesisRegistrationDtoValidator(_unitOfWork);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<ThesisRegistrationDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<ThesisRegistration>().GetAllInclude();

            if (request.facultyId != null)
            {
                query = query.Where(x => x.Group.Leader.Student.Major.Industry.FacultyId == request.facultyId);
            }

            if (request.isAllDetail || request.isGetThesis == true)
            {
                query = query.Include(x => x.Thesis);
            }

            if (request.isAllDetail || request.isGetGroup == true)
            {
                query = query.Include(x => x.Group);
            }

            int totalCount = await query.CountAsync();

            query = _sieveProcessor.Apply(sieve, query);

            var ThesisRegistrations = await query.ToListAsync();

            var mapThesisRegistrations = _mapper.Map<List<ThesisRegistrationDto>>(ThesisRegistrations);

            foreach (var thesisRegistration in mapThesisRegistrations)
            {
                // Detail infor Thesis
                if (request.isAllDetail == true || request.isGetThesis == true)
                {
                    var thesisInstructions = await _unitOfWork.Repository<ThesisInstruction>()
                                                .Query()
                                                .Where(x => x.ThesisId == thesisRegistration.ThesisId)
                                                .Include(x => x.Teacher)
                    .Select(x => x.Teacher)
                                                .ToListAsync();

                    thesisRegistration.Thesis.ThesisInstructions = _mapper.Map<List<TeacherDto>>(thesisInstructions);

                    var thesisReviews = await _unitOfWork.Repository<ThesisReview>()
                                                .Query()
                                                .Where(x => x.ThesisId == thesisRegistration.ThesisId)
                                                .Include(x => x.Teacher)
                                                .Select(x => x.Teacher)
                    .ToListAsync();

                    thesisRegistration.Thesis.ThesisReviews = _mapper.Map<List<TeacherDto>>(thesisReviews);

                    var thesisMajors = await _unitOfWork.Repository<ThesisMajor>()
                                                .Query()
                                                .Where(x => x.ThesisId == thesisRegistration.ThesisId)
                                                .Include(x => x.Major)
                                                .Select(x => x.Major)
                    .ToListAsync();

                    thesisRegistration.Thesis.ThesisMajors = _mapper.Map<List<MajorDto>>(thesisMajors);
                }

                // Detail infor Group
                if (request.isAllDetail == true || request.isGetGroup == true)
                {
                    var members = await _unitOfWork.Repository<StudentJoin>()
                                        .Query()
                                        .Where(x => x.GroupId == thesisRegistration.GroupId)
                    .Include(x => x.Student)
                                        .ToListAsync();

                    thesisRegistration.Group.Members = _mapper.Map<List<StudentJoinDto>>(members);
                }
            }

            return PaginatedResult<List<ThesisRegistrationDto>>.Success(
                mapThesisRegistrations, totalCount, request.page,
                request.pageSize);
        }
    }
}