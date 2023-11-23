using AutoMapper;
using Core.Application.DTOs.Department;
using Core.Application.DTOs.DepartmentDuty;
using Core.Application.DTOs.Faculty;
using Core.Application.DTOs.FacultyDuty;
using Core.Application.DTOs.Feedback;
using Core.Application.DTOs.Group;
using Core.Application.DTOs.Industry;
using Core.Application.DTOs.Invitation;
using Core.Application.DTOs.Major;
using Core.Application.DTOs.Notification;
using Core.Application.DTOs.RegistrationPeriod;
using Core.Application.DTOs.Student;
using Core.Application.DTOs.StudentJoin;
using Core.Application.DTOs.Teacher;
using Core.Application.DTOs.Thesis;
using Core.Application.DTOs.ThesisRegistration;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Models.Identity.Roles;
using Core.Application.Models.Identity.ViewModels;
using Core.Domain.Entities;
using Core.Domain.Entities.Identity;
using Sieve.Models;

namespace Core.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {

            CreateMap<string, List<string>>().ConvertUsing<StringToListTypeConverter>();
            CreateMap<List<string>, string>().ConvertUsing<ListToStringTypeConverter>();

            CreateMap<SieveModel, ListBaseRequest<TeacherDto>>().ReverseMap();
            CreateMap<Teacher, TeacherDto>().ReverseMap();
            CreateMap<Teacher, CreateTeacherDto>().ReverseMap();
            CreateMap<Teacher, UpdateTeacherDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<DepartmentDto>>().ReverseMap();
            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<Department, CreateDepartmentDto>().ReverseMap();
            CreateMap<Department, UpdateDepartmentDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<FacultyDto>>().ReverseMap();
            CreateMap<Faculties, FacultyDto>().ReverseMap();
            CreateMap<Faculties, CreateFacultyDto>().ReverseMap();
            CreateMap<Faculties, UpdateFacultyDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<MajorDto>>().ReverseMap();
            CreateMap<Major, MajorDto>().ReverseMap();
            CreateMap<Major, CreateMajorDto>().ReverseMap();
            CreateMap<Major, UpdateMajorDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<IndustryDto>>().ReverseMap();
            CreateMap<Industry, IndustryDto>().ReverseMap();
            CreateMap<Industry, CreateIndustryDto>().ReverseMap();
            CreateMap<Industry, UpdateIndustryDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<StudentDto>>().ReverseMap();
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<Student, CreateStudentDto>().ReverseMap();
            CreateMap<Student, UpdateStudentDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<NotificationDto>>().ReverseMap();
            CreateMap<Notification, NotificationDto>().ReverseMap();
            CreateMap<Notification, CreateNotificationDto>().ReverseMap();
            CreateMap<Notification, UpdateNotificationDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<RegistrationPeriodDto>>().ReverseMap();
            CreateMap<RegistrationPeriod, RegistrationPeriodDto>().ReverseMap();
            CreateMap<RegistrationPeriod, CreateRegistrationPeriodDto>().ReverseMap();
            CreateMap<RegistrationPeriod, UpdateRegistrationPeriodDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<StudentJoinDto>>().ReverseMap();
            CreateMap<StudentJoin, StudentJoinDto>().ReverseMap();
            CreateMap<StudentJoin, CreateStudentJoinDto>().ReverseMap();
            CreateMap<StudentJoin, UpdateStudentJoinDto>().ReverseMap();


            CreateMap<SieveModel, ListBaseRequest<DepartmentDutyDto>>().ReverseMap();
            CreateMap<DepartmentDuty, DepartmentDutyDto>().ReverseMap();
            CreateMap<DepartmentDuty, CreateDepartmentDutyDto>().ReverseMap();
            CreateMap<DepartmentDuty, UpdateDepartmentDutyDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<FacultyDutyDto>>().ReverseMap();
            CreateMap<FacultyDuty, FacultyDutyDto>().ReverseMap();
            CreateMap<FacultyDuty, CreateFacultyDutyDto>().ReverseMap();
            CreateMap<FacultyDuty, UpdateFacultyDutyDto>().ReverseMap();


            CreateMap<SieveModel, ListBaseRequest<GroupDto>>().ReverseMap();
            CreateMap<GroupDto, Group>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<InvitationDto>>().ReverseMap();
            CreateMap<Invitation, InvitationDto>().ReverseMap();
            CreateMap<Invitation, SendInvitationDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<ThesisDto>>().ReverseMap();
            CreateMap<Thesis, ThesisDto>().ReverseMap();
            CreateMap<Thesis, CreateThesisDto>().ReverseMap();
            CreateMap<Thesis, UpdateThesisDto>().ReverseMap();
            CreateMap<ThesisDto, CreateThesisDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<FeedbackDto>>().ReverseMap();
            CreateMap<Feedback, FeedbackDto>().ReverseMap();
            CreateMap<Feedback, CreateFeedbackDto>().ReverseMap();

            CreateMap<Role, RoleRequest>().ReverseMap();
            CreateMap<Role, RoleResult>().ReverseMap();

            CreateMap<User, UserVM>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<ThesisRegistrationDto>>().ReverseMap();
            CreateMap<ThesisRegistration, ThesisRegistrationDto>().ReverseMap();
            CreateMap<ThesisRegistration, CreateThesisRegistrationDto>().ReverseMap();


        }

        public class StringToListTypeConverter : ITypeConverter<string, List<string>>
        {
            public List<string> Convert(string source, List<string> destination, ResolutionContext context)
            {
                if (string.IsNullOrEmpty(source))
                {
                    return new List<string>();
                }

                return source.Split(',').ToList();
            }
        }

        public class ListToStringTypeConverter : ITypeConverter<List<string>, string>
        {
            public string Convert(List<string> source, string destination, ResolutionContext context)
            {
                if (source == null || source.Count == 0)
                {
                    return null;
                }

                return string.Join(",", source);
            }
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
