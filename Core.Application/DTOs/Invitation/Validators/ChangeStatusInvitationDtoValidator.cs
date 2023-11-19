using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Application.Transform;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using InvitationEntity = Core.Domain.Entities.Invitation;

namespace Core.Application.DTOs.Invitation.Validators
{
    public class ChangeStatusInvitationDtoValidator : AbstractValidator<ChangeStatusInvitationDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContext;

        public ChangeStatusInvitationDtoValidator(IUnitOfWork unitOfWork, int pId, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTransform.Required("id"));

            RuleFor(x => x.Status)
                 .Cascade(CascadeMode.StopOnFirstFailure) // Dừng kiểm tra khi gặp lỗi đầu tiên
                 .MustAsync(async (status, token) =>
                 {
                     var userId = _httpContext.HttpContext?.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;

                     var invitation = await _unitOfWork.Repository<InvitationEntity>().FirstOrDefaultAsync(x => x.Id == pId);

                     var isReceiver = await _unitOfWork.Repository<InvitationEntity>()
                                         .GetByIdInclude(pId)
                                         .Include(x => x.StudentJoin)
                                             .ThenInclude(x => x.Student)
                                         .Where(x => x.StudentJoin.Student.UserId == int.Parse(userId))
                                         .AnyAsync();
                     if (isReceiver == false && status == InvitationEntity.STATUS_ACCEPT)
                     {
                         return false;
                     }

                     var isSender = await _unitOfWork.Repository<InvitationEntity>()
                                         .GetAllInclude()
                                         .Where(x => x.GroupId == invitation.GroupId && x.Status == InvitationEntity.STATUS_CANCEL)
                                         .Include(x => x.StudentJoin)
                                             .ThenInclude(x => x.Student)
                                         .Where(x => x.StudentJoin.Student.UserId == int.Parse(userId))
                                         .AnyAsync();
                     if (isSender == false && isReceiver == false)
                     {
                         return false;
                     }

                     if (invitation?.Status == InvitationEntity.STATUS_SENT)
                     {
                         return status == InvitationEntity.STATUS_ACCEPT || status == InvitationEntity.STATUS_CANCEL;
                     }

                     return false;
                 })
                 .WithMessage(status =>
                 {
                     switch (status.Status)
                     {
                         case InvitationEntity.STATUS_ACCEPT:
                             return "Bạn không thể chấp nhận lời mời của người khác!";
                         case InvitationEntity.STATUS_CANCEL:
                             return "Bạn không thể hủy lời mời của nhóm khác!";
                         default:
                             return ValidatorTransform.MustIn("status");
                     }
                 });
        }
    }
}
