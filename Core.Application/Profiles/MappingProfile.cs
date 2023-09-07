using Core.Application.DTOs.Teacher;
using Core.Domain.Entities;
using AutoMapper;
using System.Reflection;
using Sieve.Models;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.DTOs.Department;

namespace Core.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<SieveModel, ListBaseRequest<TeacherDto>>().ReverseMap();
            CreateMap<Teacher, TeacherDto>().ReverseMap();
            CreateMap<Teacher, CreateTeacherDto>().ReverseMap();
            CreateMap<Teacher, UpdateTeacherDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<DepartmentDto>>().ReverseMap();
            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<Department, CreateDepartmentDto>().ReverseMap();
            CreateMap<Department, UpdateDepartmentDto>().ReverseMap();
        }

        public void ConfigureIgnoreFields<TEntity, TDto>(IMappingExpression<TEntity, TDto> mapping)
        {
            //mapping.ForMember(dest => dest.DateCreated, opt => opt.Ignore())
            //       .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            //       .ForMember(dest => dest.LastModifiedDate, opt => opt.Ignore())
            //       .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore())
            //       .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }

        /*
            var teacherDtos = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.Name.EndsWith("TeacherDto"))
                .ToList();
            foreach (var dto in teacherDtos)
            {
                CreateMap(typeof(Teacher), dto).ReverseMap();
            }
        

         */
    }
}
