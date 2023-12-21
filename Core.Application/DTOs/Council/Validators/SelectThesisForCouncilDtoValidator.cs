using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using CouncilEntity = Core.Domain.Entities.Council;
using ThesisEntity = Core.Domain.Entities.Thesis;

namespace Core.Application.DTOs.Council.Validators
{
    public class SelectThesisForCouncilDtoValidator : AbstractValidator<SelectThesisForCouncilDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SelectThesisForCouncilDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.CouncilId)
                .MustAsync(async (id, token) =>
                {
                    var exists = await _unitOfWork.Repository<CouncilEntity>().GetByIdAsync(id);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.NotExistsValueInTable("councilId", "councils"));

            RuleFor(x => x.ListThesis)
                .MustAsync(async (listThesis, token) =>
                {
                    if (listThesis == null || listThesis.Any() == false)
                    {
                        return false;
                    }

                    // Kiểm tra sự tồn tại của từng ThesisId trong danh sách
                    foreach (var thesis in listThesis)
                    {
                        var exists = await _unitOfWork.Repository<ThesisEntity>().GetByIdAsync(thesis.ThesisId);
                        if (exists == null)
                        {
                            return false;
                        }
                        return DateTime.Now < thesis.TimeStart && thesis.TimeStart < thesis.TimeEnd;
                    }

                    return true; // Nếu tất cả là hợp lệ, trả về true
                })
                .WithMessage(id => ValidatorTransform.NotExistsValueInTable("thesisId", "theses") + 
                            " Thời gian bắt đầu phải lớn hơn hiện tại và nhỏ hơn thời gian kết thúc");


        }
    }
}
