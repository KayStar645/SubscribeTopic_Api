using Core.Application.DTOs.Teacher;
using Core.Domain.Entities;
using AutoMapper;
using System.Reflection;
using Sieve.Models;
using Core.Application.Features.Base.Requests.Queries;

namespace Core.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            var dtos = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.Name.EndsWith("TeacherDto"))
                .ToList();

            foreach (var dto in dtos)
            {
                CreateMap(typeof(Teacher), dto).ReverseMap();
            }

            CreateMap<SieveModel, GetListRequest>().ReverseMap();

            //CreateMap<Teacher, TeacherDto>().ReverseMap();
            //CreateMap<Teacher, ListTeacherDto>().ReverseMap();
            //CreateMap<Teacher, CreateTeacherDto>().ReverseMap();
            //CreateMap<Teacher, UpdateTeacherDto>().ReverseMap();
        }
    }
}
