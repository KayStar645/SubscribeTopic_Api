using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using ExchangeEntity = Core.Domain.Entities.Exchange;

namespace Core.Application.Features.Exchanges.Events
{
    public class BeforeCreateExchangeUpdateExchangeEvent : INotification
    {
        public ExchangeEntity _exchange { get; set; }

        public IHttpContextAccessor _httpContextAccessor;

        public IUnitOfWork _unitOfWork;

        public BeforeCreateExchangeUpdateExchangeEvent(ExchangeEntity exchange,
            IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _exchange = exchange;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
    }

    public class BeforeCreateExchangeUpdateExchangeHandler : INotificationHandler<BeforeCreateExchangeUpdateExchangeEvent>
    {
        public async Task Handle(BeforeCreateExchangeUpdateExchangeEvent pEvent, CancellationToken cancellationToken)
        {
            try
            {
                var userId = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
                var userType = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;

                if (userType == CLAIMS_VALUES.TYPE_TEACHER)
                {

                    // Từ id của người dùng lấy ra id của giáo viên
                    var teacher = await pEvent._unitOfWork.Repository<Teacher>()
                        .FirstOrDefaultAsync(x => x.UserId == int.Parse(userId));

                    pEvent._exchange.TeacherId = teacher.Id;
                }
                else if (userType == CLAIMS_VALUES.TYPE_STUDENT)
                {
                    // Từ id của người dùng lấy ra id của sinh viên
                    var student = await pEvent._unitOfWork.Repository<Student>()
                        .FirstOrDefaultAsync(x => x.UserId == int.Parse(userId));

                    pEvent._exchange.StudentId = student.Id;
                }    
                else
                {
                    throw new UnauthorizedException(StatusCodes.Status403Forbidden);
                }

            }
            catch (UnauthorizedException ex)
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}