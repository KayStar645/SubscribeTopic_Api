using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Thesis;
using Core.Application.Exceptions;
using Core.Application.Features.Thesiss.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Core.Application.Features.Thesiss.Handlers.Commands
{
    public class ApproveThesisCommandHandler : IRequestHandler<ApproveThesisRequest, Result<ThesisDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ApproveThesisCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ThesisDto>> Handle(ApproveThesisRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var findThesis = await _unitOfWork.Repository<Thesis>().GetByIdAsync(request.approveThesisDto.Id);

                if (findThesis is null)
                {
                    return Result<ThesisDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.approveThesisDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }
                if(findThesis.Status != Thesis.STATUS_APPROVE_REQUEST)
                {
                    throw new UnauthorizedException(StatusCodes.Status403Forbidden);
                }    

                findThesis.Status = Thesis.STATUS_APPROVED;
                var newThesis = await _unitOfWork.Repository<Thesis>().UpdateAsync(findThesis);
                await _unitOfWork.Save(cancellationToken);

                var thesisDto = _mapper.Map<ThesisDto>(newThesis);

                return Result<ThesisDto>.Success(thesisDto, (int)HttpStatusCode.OK);
            }
            catch (NotFoundException ex)
            {
                return Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (BadRequestException ex)
            {
                return Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
            catch (UnauthorizedException ex)
            {
                return Result<ThesisDto>.Failure(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
