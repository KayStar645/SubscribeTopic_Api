using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Teacher;
using Core.Application.Features.Teachers.Requests.Queries;
using MediatR;
using AutoMapper;

namespace Core.Application.Features.Teachers.Handlers.Queries
{
    public class GetTeacherDetailRequestHandlers : IRequestHandler<GetTeacherDetailRequest, TeacherDto>
    {
        private readonly ITeacherRepository _teacherRepo;
        private readonly IMapper _mapper;

        public GetTeacherDetailRequestHandlers(ITeacherRepository teacherRepository, IMapper mapper) 
        {
            _teacherRepo = teacherRepository;
            _mapper = mapper;
        }

        public async Task<TeacherDto> Handle(GetTeacherDetailRequest request, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepo.GetByIdAsync(request.Id);
            return _mapper.Map<TeacherDto>(teacher);
        }
    }
}
