using Core.Application.DTOs.Teacher;
using Core.Domain.Entities;
using AutoMapper;

namespace Core.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {

            CreateMap<Teacher, TeacherDto>().ReverseMap();
            CreateMap<Teacher, TeacherListDto>().ReverseMap();
            CreateMap<Teacher, CreateTeacherDto>().ReverseMap();
            CreateMap<Teacher, UpdateTeacherDto>().ReverseMap();
        }
    }
}
