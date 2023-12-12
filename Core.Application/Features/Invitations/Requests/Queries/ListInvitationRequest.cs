using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Invitation;
using Core.Application.Features.Base.Requests.Queries;
using FluentValidation;

namespace Core.Application.Features.Invitations.Requests.Queries
{
    public class ListInvitationRequest : ListBaseRequest<InvitationDto>
    {
        public bool isGetGroup { get; set; }

        public bool isGetStudentJoin { get; set; }
    }

    public class ListInvitationDtoValidator : AbstractValidator<ListInvitationRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListInvitationDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<InvitationDto>());
        }
    }
}
