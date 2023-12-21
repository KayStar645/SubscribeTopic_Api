using Core.Application.Contracts.Persistence;
using Core.Application.Services;
using Core.Application.Transform;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TeacherEntity = Core.Domain.Entities.Teacher;
using CommissionerEntity = Core.Domain.Entities.Commissioner;

namespace Core.Application.DTOs.Council.Validators
{
    public class CouncilDtoValidator : AbstractValidator<ICouncilDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CouncilDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage(ValidatorTransform.Required("location"))
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("location", 500));

            RuleFor(x => x.Commissioners)
                .MustAsync(async (commissioners, cancellationToken) =>
                {
                    if (commissioners == null)
                    {
                        return true;
                    }

                    foreach (var commissioner in commissioners)
                    {
                        // Kiểm tra TeacherId
                        if ((await _unitOfWork.Repository<TeacherEntity>().Query()
                            .AnyAsync(t => t.Id == commissioner.TeacherId)) == false)
                        {
                            return false;
                        }

                        // Kiểm tra Position
                        if (CommissionerEntity.GetPosition().Contains(commissioner.Position) == false)
                        {
                            return false;
                        }

                        // Kiểm tra điều kiện đặc biệt cho Position "C" và "S"
                        if (commissioner.Position == CommissionerEntity.POSITION_CHAIRPERSON &&
                             commissioners.Count(c => c.Position == CommissionerEntity.POSITION_CHAIRPERSON) > 1)
                        {
                            return false;
                        }
                        if (commissioner.Position == CommissionerEntity.POSITION_SECRETARY &&
                             commissioners.Count(c => c.Position == CommissionerEntity.POSITION_SECRETARY) > 1)
                        {
                            return false;
                        }
                    }

                    return true;
                })
                .WithMessage("Mỗi giáo viên phải có id hợp lệ và có type là: " +
                    string.Join(",", CommissionerEntity.GetPosition()) + ", 1C, 1S");



        }
    }
}
