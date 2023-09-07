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

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"));

            RuleFor(x => x.Name)
                .NotNull().WithMessage(ValidatorTranform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTranform.MaximumLength("name", 190));

            RuleFor(x => x.Gender)
                .Must(gender => gender == CommonTranform.male || gender == CommonTranform.female || gender == CommonTranform.other)
                .WithMessage(ValidatorTranform.Must("gender", CommonTranform.GetGender()));

            RuleFor(x => x.DateOfBirth)
                .Must(dateOfBirth => CustomValidator.IsAtLeastNYearsOld(dateOfBirth, 16))
                .WithMessage(ValidatorTranform.MustDate("dateOfBirth", 16));

            RuleFor(x => x.PhoneNumber)
                .Must(phoneNumber => string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length == 10)
                .WithMessage(ValidatorTranform.Length("phoneNumber", 10));

            RuleFor(x => x.Email)
                .Must(email => string.IsNullOrEmpty(email) || CustomValidator.BeValidEmail(email))
                .WithMessage(ValidatorTranform.ValidValue("email"));

            RuleFor(x => x.AcademicTitle)
                .Must(academicTitle => string.IsNullOrEmpty(academicTitle) || academicTitle == CommonTranform.bachelor || academicTitle == CommonTranform.engineer ||
                academicTitle == CommonTranform.postgraduate || academicTitle == CommonTranform.master || academicTitle == CommonTranform.doctorate)
                .WithMessage(ValidatorTranform.Must("academicTitle", CommonTranform.GetListAcademicTitle()));

            RuleFor(x => x.Degree)
                .Must((dto, degree) =>
                    string.IsNullOrEmpty(dto.AcademicTitle) || dto.AcademicTitle != CommonTranform.doctorate
                    ? string.IsNullOrEmpty(degree)
                    : degree == CommonTranform.associateProfessor || degree == CommonTranform.professor || string.IsNullOrWhiteSpace(degree))
                .WithMessage(ValidatorTranform.MustWhen("degree", CommonTranform.GetListDegree(), "academicTitle", CommonTranform.doctorate));
        }    
    }
}
