using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Commissioner;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Council;
using Core.Application.Features.Councils.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.Councils.Handlers.Queries
{
    public class DetailCouncilQueryHandler : IRequestHandler<DetailCouncilRequest, Result<CouncilDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DetailCouncilQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CouncilDto>> Handle(DetailCouncilRequest request, CancellationToken cancellationToken)
        {
            var validator = new DetailBaseRequestValidator();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<CouncilDto>.Failure(string.Join(", ", errorMessages), (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var query = _unitOfWork.Repository<Council>().GetByIdInclude(request.id);

                var findCouncil = await query.SingleOrDefaultAsync();

                if (findCouncil is null)
                {
                    return Result<CouncilDto>.Failure(
                        ValidatorTransform.NotExistsValue("id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var councilDto = _mapper.Map<CouncilDto>(findCouncil);

                var com = await _unitOfWork.Repository<Commissioner>()
                            .Query().Where(x => x.CouncilId == councilDto.Id)
                            .Include(x => x.Teacher)
                            .ToListAsync();
                councilDto.Commissioners = _mapper.Map<List<CommissionerDto>>(com);

                return Result<CouncilDto>.Success(councilDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<CouncilDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
