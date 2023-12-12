using AutoMapper;
using Core.Application.DTOs.Common;
using Core.Application.DTOs.Department;
using Core.Application.DTOs.Duty;
using Core.Application.DTOs.Exchanges;
using Core.Application.DTOs.Faculty;
using Core.Application.DTOs.Feedback;
using Core.Application.DTOs.Group;
using Core.Application.DTOs.Industry;
using Core.Application.DTOs.Invitation;
using Core.Application.DTOs.Job;
using Core.Application.DTOs.JobResults;
using Core.Application.DTOs.Major;
using Core.Application.DTOs.Notification;
using Core.Application.DTOs.RegistrationPeriod;
using Core.Application.DTOs.ReportSchedule;
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
        protected static IGoogleDriveService? _googleDriveService;

        public MappingProfile(IGoogleDriveService googleDriveService)
        {
            _googleDriveService = googleDriveService;


            CreateMap<string, List<string>>().ConvertUsing<StringToListTypeConverter>();
            CreateMap<List<string>, string>().ConvertUsing<ListToStringTypeConverter>();

            CreateMap<string, FileDto>().ConvertUsing<StringToFileDtoConverter>();
            CreateMap<FileDto, string>().ConvertUsing<FileDtoToStringConvert>();

            CreateMap<string, List<FileDto>>().ConvertUsing<StringToFileDtoListConverter>();
            CreateMap<List<FileDto>, string>().ConvertUsing<FileDtoListToStringConvert>();


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
            CreateMap<SieveModel, ListBaseRequest<FriendDto>>().ReverseMap();
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<Student, FriendDto>().ReverseMap();
            CreateMap<Student, CreateStudentDto>().ReverseMap();
            CreateMap<Student, UpdateStudentDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<NotificationDto>>().ReverseMap();
            CreateMap<Notification, CreateNotificationDto>().ReverseMap();
            CreateMap<Notification, UpdateNotificationDto>().ReverseMap();
            CreateMap<Notification, NotificationDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<RegistrationPeriodDto>>().ReverseMap();
            CreateMap<RegistrationPeriod, RegistrationPeriodDto>().ReverseMap();
            CreateMap<RegistrationPeriod, CreateRegistrationPeriodDto>().ReverseMap();
            CreateMap<RegistrationPeriod, UpdateRegistrationPeriodDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<StudentJoinDto>>().ReverseMap();
            CreateMap<StudentJoin, StudentJoinDto>().ReverseMap();
            CreateMap<StudentJoin, CreateStudentJoinDto>().ReverseMap();
            CreateMap<StudentJoin, UpdateStudentJoinDto>().ReverseMap();

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

            CreateMap<SieveModel, ListBaseRequest<JobDto>>().ReverseMap();
            CreateMap<Job, JobDto>().ReverseMap();
            CreateMap<Job, CreateJobDto>().ReverseMap();
            CreateMap<Job, UpdateJobDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<JobResultsDto>>().ReverseMap();
            CreateMap<JobResults, JobResultsDto>().ReverseMap();
            CreateMap<JobResults, SubmitJobResultsDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<ExchangeDto>>().ReverseMap();
            CreateMap<Exchange, ExchangeDto>().ReverseMap();
            CreateMap<Exchange, CreateExchangeDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<ReportScheduleDto>>().ReverseMap();
            CreateMap<ReportSchedule, ReportScheduleDto>().ReverseMap();
            CreateMap<ReportSchedule, CreateReportScheduleDto>().ReverseMap();
            CreateMap<ReportSchedule, UpdateReportScheduleDto>().ReverseMap();

            CreateMap<SieveModel, ListBaseRequest<DutyDto>>().ReverseMap();
            CreateMap<Duty, DutyDto>().ReverseMap();
            CreateMap<Duty, CreateDutyDto>().ReverseMap();
            CreateMap<Duty, UpdateDutyDto>().ReverseMap();


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

        public class StringToFileDtoConverter : ITypeConverter<string, FileDto>
        {
            public FileDto Convert(string source, FileDto destination, ResolutionContext context)
            {
                if (string.IsNullOrEmpty(source))
                {
                    return new FileDto();
                }

                var result = _googleDriveService.GetFileInfoFromGoogleDrive(source).Result;
                return result.Data ?? new FileDto();
            }
        }

        public class FileDtoToStringConvert : ITypeConverter<FileDto, string>
        {
            public string Convert(FileDto source, string destination, ResolutionContext context)
            {
                return source?.Path ?? string.Empty;
            }
        }

        public class StringToFileDtoListConverter : ITypeConverter<string, List<FileDto>>
        {
            public List<FileDto> Convert(string source, List<FileDto> destination, ResolutionContext context)
            {
                if (string.IsNullOrEmpty(source))
                {
                    return new List<FileDto>();
                }

                var imagePaths = source.Split(',');
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
        }

        public class FileDtoListToStringConvert : ITypeConverter<List<FileDto>, string>
        {
            public string Convert(List<FileDto> source, string destination, ResolutionContext context)
            {
                if (source == null || source.Count == 0)
                {
                    return null;
                }

                var paths = source.Select(fileDto => fileDto.Path);
                return string.Join(",", paths);
            }
        }
    }
}
