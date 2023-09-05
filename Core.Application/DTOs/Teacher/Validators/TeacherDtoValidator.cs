using FluentValidation;
using Core.Application.Custom;
using Core.Application.Transform;

namespace Core.Application.DTOs.Teacher.Validators
{
    public class TeacherDtoValidator : AbstractValidator<ITeacherDto>
    {

        public TeacherDtoValidator()
        {
            // Ràng buộc chuyên ngành, không có khoa
            //RuleFor(p => p.FacultyId)
            //.Must(facultyId => ValidFacultyIds.Contains(facultyId))
            //.WithMessage("Invalid FacultyId.");

            RuleFor(p => p.Name)
                .NotNull().WithMessage(ValidatorTranform.Required("Name"))
                .MinimumLength(5).WithMessage(ValidatorTranform.MinimumLength("Name", 5))
                .MaximumLength(190).WithMessage(ValidatorTranform.MaximumLength("Name", 190));

            //RuleFor(p => p.Gender)
            //    .Must(gender => gender == CommonTranform.male || gender == CommonTranform.female || gender == CommonTranform.other)
            //    .WithMessage(ValidatorTranform.MustIn("Gender"));
                
            //RuleFor(p => p.DateOfBirth)
            //    .Must(dateOfBirth => CustomValidator.IsAtLeastNYearsOld(dateOfBirth, 16))
            //    .WithMessage(ValidatorTranform.MustDate("DateOfBirth", 16));

            //RuleFor(p => p.PhoneNumber)
            //    .Length(10).WithMessage(ValidatorTranform.Length("PhoneNumber", 10));

            //RuleFor(p => p.Email)
            //    .Null()
            //    .Must(email => CustomValidator.BeValidEmail(email))
            //    .WithMessage(ValidatorTranform.ValidValue("email"));

            //RuleFor(p => p.AcademicTitle)
            //    .Null()
            //    .Must(academicTitle => academicTitle == CommonTranform.bachelor || academicTitle == CommonTranform.engineer ||
            //    academicTitle == CommonTranform.postgraduate || academicTitle == CommonTranform.master || academicTitle == CommonTranform.doctorate)
            //    .WithMessage(ValidatorTranform.MustIn("AcademicTitle"));

            //RuleFor(p => p.Degree)
            //    .Null()
            //    .Must((dto, degree) =>
            //        string.IsNullOrWhiteSpace(dto.AcademicTitle) || dto.AcademicTitle != CommonTranform.doctorate
            //        ? string.IsNullOrWhiteSpace(degree)
            //        : degree == CommonTranform.associateProfessor || degree == CommonTranform.professor || string.IsNullOrWhiteSpace(degree))
            //    .WithMessage(ValidatorTranform.MustWhen("degree", CommonTranform.GetListDegree(), "AcademicTitle", CommonTranform.professor));
        }    
    }
}
