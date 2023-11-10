using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Thesis;
using Core.Application.DTOs.Thesis.Validators;
using Core.Application.Features.Thesiss.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.Thesiss.Handlers.Commands
{
    public class ChangeStatusThesisCommandHandler : IRequestHandler<ChangeStatusThesisRequest, Result<ThesisDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChangeStatusThesisCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ThesisDto>> Handle(ChangeStatusThesisRequest request, CancellationToken cancellationToken)
        {
            var validator = new ChangeStatusThesisDtoValidator(_unitOfWork, (int)request.changeStatusThesis.Id);
            var validationResult = await validator.ValidateAsync(request.changeStatusThesis);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<ThesisDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var findThesis = await _unitOfWork.Repository<Thesis>().GetByIdAsync(request.changeStatusThesis.Id);

                if (findThesis is null)
                {
                    return Result<ThesisDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.changeStatusThesis.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                findThesis.CopyPropertiesFrom(request.changeStatusThesis);

                var newThesis = await _unitOfWork.Repository<Thesis>().UpdateAsync(findThesis);
                await _unitOfWork.Save(cancellationToken);

                var thesisDto = _mapper.Map<ThesisDto>(newThesis);

                return Result<ThesisDto>.Success(thesisDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
