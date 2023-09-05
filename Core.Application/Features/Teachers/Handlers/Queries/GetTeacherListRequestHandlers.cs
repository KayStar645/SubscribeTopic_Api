using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Teacher;
using Core.Application.Extensions;
using Core.Application.Features.Teachers.Requests.Queries;
using Core.Domain.Entities;
using MediatR;
using Shared;

namespace Core.Application.Features.Teachers.Handlers.Queries
{
    public class GetTeacherListRequestHandlers : IRequestHandler<GetTeacherListRequest, PaginatedResult<TeacherListDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTeacherListRequestHandlers(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PaginatedResult<TeacherListDto>> Handle(GetTeacherListRequest request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Repository<Teacher>().Entities    
                    .OrderBy(x => x.Name)
                    .ProjectTo<TeacherListDto>(_mapper.ConfigurationProvider)
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}
