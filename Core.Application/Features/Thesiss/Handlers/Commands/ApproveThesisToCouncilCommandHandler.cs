using AutoMapper;
using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Thesis;
using Core.Application.Exceptions;
using Core.Application.Features.Thesiss.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.Thesiss.Handlers.Commands
{
    public class ApproveThesisToCouncilCommandHandler : IRequestHandler<ApproveThesisToCouncilRequest, Result<ThesisDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _httpContext;

        public ApproveThesisToCouncilCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, 
            IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        public async Task<Result<ThesisDto>> Handle(ApproveThesisToCouncilRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
                var userType = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;

                if (userType != CLAIMS_VALUES.TYPE_TEACHER)
                {
                    throw new UnauthorizedException(StatusCodes.Status403Forbidden);
                }

                // Từ id của người dùng lấy ra id của giáo viên
                var teacher = await _unitOfWork.Repository<Teacher>()
                    .FirstOrDefaultAsync(x => x.UserId == int.Parse(userId));

                var findIns = await _unitOfWork.Repository<ThesisInstruction>()
                                    .Query()
                                    .Where(x => x.ThesisId == request.ThesisId && x.TeacherId == teacher.Id)
                                    .FirstOrDefaultAsync();
                var findRev = await _unitOfWork.Repository<ThesisReview>()
                                    .Query()
                                    .Where(x => x.ThesisId == request.ThesisId && x.TeacherId == teacher.Id)
                                    .FirstOrDefaultAsync();

                if (findIns is null && findRev is null)
                {
                    return Result<ThesisDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.ThesisId.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                if(findIns is null)
                {
                    findRev.IsApprove = true;
                    await _unitOfWork.Repository<ThesisReview>().UpdateAsync(findRev);
                }    
                else
                {
                    findIns.IsApprove = true;
                    await _unitOfWork.Repository<ThesisInstruction>().UpdateAsync(findIns);
                }    
                await _unitOfWork.Save(cancellationToken);

                var findThesis = await _unitOfWork.Repository<Thesis>().GetByIdAsync(request.ThesisId);

                var thesisDto = _mapper.Map<ThesisDto>(findThesis);

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
