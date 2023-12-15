using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Thesis;
using Core.Application.Features.Base.Requests.Queries;
using FluentValidation;

namespace Core.Application.Features.Thesiss.Requests.Queries
{
    public class ListThesisRegistrationRequest : ListBaseRequest<ThesisRegisteredDto>
    {
        public bool? isGetAll { get; set; }
    }

    public class ListThesisRegistrationValidator : AbstractValidator<ListThesisRegistrationRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListThesisRegistrationValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<ThesisRegisteredDto>());
        }
    }
}
