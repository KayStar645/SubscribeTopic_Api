using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Faculty;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Common;
using MediatR;
using System.Net;

namespace Core.Application.Features.Base.Handlers.Commands
{
    public class DeleteBaseCommandHandler<T> : IRequestHandler<DeleteBaseRequest<T>, Unit>
        where T : BaseAuditableEntity
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteBaseCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteBaseRequest<T> request, CancellationToken cancellationToken)
        {
            var validator = new DeleteBaseRequestValidator<T>();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                throw new BadRequestException(string.Join(",", errorMessages));
            }

            var entity = await _unitOfWork.Repository<T>().GetByIdAsync(request.id);

            if (entity == null)
                throw new NotFoundException("id", request.id.ToString());

            await _unitOfWork.Repository<T>().DeleteAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }
    }
}
