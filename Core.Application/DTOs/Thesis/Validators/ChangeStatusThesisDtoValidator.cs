using Core.Application.Transform;
using FluentValidation;
using ThesisEntity = Core.Domain.Entities.Thesis;

namespace Core.Application.DTOs.Thesis.Validators
{
    public class ChangeStatusThesisDtoValidator : AbstractValidator<ChangeStatusThesisDto>
    {
        public ChangeStatusThesisDtoValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage(ValidatorTransform.Required("status"))
                .Must(status => ThesisEntity.GetSatus().Contains(status)).WithMessage(ValidatorTransform.Exists("status"))
                .MustAsync(async (status, token) =>
                {
                    var isValid = await ValidateStatusTransitionAsync(status);
                    return isValid;
                }).WithMessage(ValidatorTransform.Exists("status"));
        }

        private async Task<bool> ValidateStatusTransitionAsync(string status)
        {
            var validTransitions = new Dictionary<string, List<string>>
            {
                { ThesisEntity.STATUS_DRAFT, new List<string> { ThesisEntity.STATUS_APPROVE_REQUEST } },
                { ThesisEntity.STATUS_APPROVE_REQUEST, new List<string> { ThesisEntity.STATUS_EDITING_REQUEST, ThesisEntity.STATUS_APPROVED, ThesisEntity.STATUS_CANCEL } },
                { ThesisEntity.STATUS_EDITING_REQUEST, new List<string> { ThesisEntity.STATUS_APPROVE_REQUEST } }
            };

            if (validTransitions.TryGetValue(status, out var validNextStatusList))
            {
                // Kiểm tra xem `status` có hợp lệ theo nguyên tắc không
                // Trong trường hợp này, `status` phải nằm trong danh sách `validNextStatusList`
                return validNextStatusList.Contains(status);
            }

            // Nếu không tìm thấy quy tắc cho `status`, mặc định trả về false
            return false;
        }

    }
}

/*
        public const string STATUS_DRAFT = "D";
        public const string STATUS_APPROVE_REQUEST = "AR";
        public const string STATUS_EDITING_REQUEST = "ER";
        public const string STATUS_APPROVED = "A";
        public const string STATUS_CANCEL = "C";

        D  -> AR
        AR -> ER/A/C
        ER -> AR
 */
