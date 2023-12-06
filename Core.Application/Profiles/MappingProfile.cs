using AutoMapper;
using Core.Application.DTOs.Common;
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
using Core.Application.Interfaces.Services;
using Core.Application.Models.Identity.Roles;
using Core.Application.Models.Identity.ViewModels;
using Core.Domain.Entities;
using Core.Domain.Entities.Identity;
using Sieve.Models;

namespace Core.Application.Profiles
{
    public class MappingProfile : Profile
    {
        private IGoogleDriveService? _googleDriveService;

        public MappingProfile(IGoogleDriveService googleDriveService) 
        {
            _googleDriveService = googleDriveService;


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
            CreateMap<Notification, CreateNotificationDto>().ReverseMap();
            CreateMap<Notification, UpdateNotificationDto>().ReverseMap();
            //CreateMap<Notification, NotificationDto>().ReverseMap(); // Tại đây
            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(
                    src => MapImageStringToDto(src.Image))
                )
                .ForMember(dest => dest.Images, opt => opt.MapFrom(
                    src => MapImagesStringToList(src.Images))
                );

            CreateMap<NotificationDto, Notification>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(
                    src => MapImageDtoToString(src.Image))
                )
                .ForMember(dest => dest.Images, opt => opt.MapFrom(
                    src => MapImagesListToString(src.Images))
                );


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

        private List<FileDto> MapImagesStringToList(string? imagesString)
        {
            if (string.IsNullOrEmpty(imagesString))
            {
                return new List<FileDto>();
            }

            var imagePaths = imagesString.Split(',');
            var fileDtos = new List<FileDto>();

            foreach (var imagePath in imagePaths)
            {
                var result = _googleDriveService.GetFileInfoFromGoogleDrive(imagePath).Result;
                if (result.Data != null)
                {
                    fileDtos.Add(result.Data);
                }
            }

            return fileDtos;
        }

        private string MapImagesListToString(List<FileDto>? imagesList)
        {
            if (imagesList == null || imagesList.Count == 0)
            {
                return null;
            }

            var imagePaths = imagesList.Select(fileDto => fileDto.Path);
            return string.Join(",", imagePaths);
        }

        private FileDto MapImageStringToDto(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return new FileDto();
            }
            var result = _googleDriveService.GetFileInfoFromGoogleDrive(imagePath).Result;
            return result.Data ?? new FileDto();
        }

        private string MapImageDtoToString(FileDto? imageDto)
        {
            return imageDto?.Path ?? string.Empty;
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
