﻿using AutoMapper;
using Core.Application.DTOs.Department;
using Core.Application.DTOs.Faculty;
using Core.Application.DTOs.Major;
using Core.Application.DTOs.Student;
using Core.Application.DTOs.Teacher;
using Core.Application.Features.Base.Requests.Queries;
using Core.Domain.Entities;
using Sieve.Models;

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

            CreateMap<SieveModel, ListBaseRequest<FacultyDto>>().ReverseMap();
            CreateMap<Faculty, FacultyDto>().ReverseMap();
            CreateMap<Faculty, CreateFacultyDto>().ReverseMap();
            CreateMap<Faculty, UpdateFacultyDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<MajorDto>>().ReverseMap();
            CreateMap<Major, MajorDto>().ReverseMap();
            CreateMap<Major, CreateMajorDto>().ReverseMap();
            CreateMap<Major, UpdateMajorDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<StudentDto>>().ReverseMap();
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<Student, CreateStudentDto>().ReverseMap();
            CreateMap<Student, UpdateStudentDto>().ReverseMap();
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
