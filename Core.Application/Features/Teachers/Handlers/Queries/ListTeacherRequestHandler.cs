using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Teacher;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Teachers.Handlers.Queries
{
    public class ListTeacherRequestHandler : IRequestHandler<ListBaseRequest<TeacherDto>, PaginatedResult<List<TeacherDto>>>
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
        public async Task<PaginatedResult<List<TeacherDto>>> Handle(ListBaseRequest<TeacherDto> request, CancellationToken cancellationToken)
        {
            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Teacher>().GetAllSieveAsync();

            query = _sieveProcessor.Apply(sieve, query);

            var teachers = await query.ToListAsync();

            var mapTeachers = _mapper.Map<List<TeacherDto>>(teachers);
            return PaginatedResult<List<TeacherDto>>.Create(
                mapTeachers, 0, request.Page,
                request.PageSize, (int)HttpStatusCode.OK);
        }
    }
}
