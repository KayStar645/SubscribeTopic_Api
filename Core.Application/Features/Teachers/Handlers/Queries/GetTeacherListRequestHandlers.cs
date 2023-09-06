using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Teacher;
using Core.Application.Extensions;
using Core.Application.Features.Base.Requests.Queries;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared;
using Sieve.Models;
using Sieve.Services;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Teachers.Handlers.Queries
{
    public class GetTeacherListRequestHandlers : IRequestHandler<GetListRequest, PaginatedResult<List<ListTeacherDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;

        public GetTeacherListRequestHandlers(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }
        public async Task<PaginatedResult<List<ListTeacherDto>>> Handle(GetListRequest request, CancellationToken cancellationToken)
        {
            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Teacher>().GetAllSieveAsync();

            query = _sieveProcessor.Apply(sieve, query);

            var teachers = await query.ToListAsync();

            var mapTeachers = _mapper.Map<List<ListTeacherDto>>(teachers);
            return PaginatedResult<List<ListTeacherDto>>.Create(
                mapTeachers, 0, request.Page,
                request.PageSize, (int)HttpStatusCode.OK);
        }
    }
}
