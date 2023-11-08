using Core.Application.Constants;
using Core.Application.Interfaces.Repositories;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.Features.Thesiss.Events
{
    public class BeforeCreateThesisUpdateThesisEvent : INotification
    {
        public Thesis _thesis { get; set; }

        public IHttpContextAccessor _httpContextAccessor;
        public IUserRepository _userRepository { get; set; }

        public BeforeCreateThesisUpdateThesisEvent(Thesis thesis, 
            IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            _thesis = thesis;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }
    }

    public class BeforeCreateThesisUpdateThesisHandler : INotificationHandler<BeforeCreateThesisUpdateThesisEvent>
    {
        public async Task Handle(BeforeCreateThesisUpdateThesisEvent pEvent, CancellationToken cancellationToken)
        {
            try
            {
                var x = pEvent._httpContextAccessor.HttpContext.User;

                var username = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;

                int a = 1;

                // Status = D

                // Type = tùy thuộc vào người tạo

                // Người tạo = tùy thuộc vào type

                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }

}
