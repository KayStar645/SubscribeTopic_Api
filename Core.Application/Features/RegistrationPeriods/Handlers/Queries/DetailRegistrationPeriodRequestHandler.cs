using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.RegistrationPeriod;
using Core.Application.Features.RegistrationPeriods.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.RegistrationPeriods.Handlers.Queries
{
    public class DetailRegistrationPeriodRequestHandler : IRequestHandler<DetailRegistrationPeriodRequest, Result<RegistrationPeriodDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailRegistrationPeriodRequestHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<RegistrationPeriodDto>> Handle(DetailRegistrationPeriodRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _unitOfWork.Repository<RegistrationPeriod>().GetByIdInclude(request.Id);

                if (request.IsAllDetail)
                {
                    query = _unitOfWork.Repository<RegistrationPeriod>().AddInclude(query, x => x.Faculty);
                }
                else
                {
                    if (request.IsGetFaculty == true)
                    {
                        query = _unitOfWork.Repository<RegistrationPeriod>().AddInclude(query, x => x.Faculty);
                    }
                }

                var findPeriod = await query.SingleAsync();

                if (findPeriod is null)
                {
                    return Result<RegistrationPeriodDto>.Failure(
                        ValidatorTranform.NotExistsValue("Id", request.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var periodDto = _mapper.Map<RegistrationPeriodDto>(findPeriod);

                return Result<RegistrationPeriodDto>.Success(periodDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<RegistrationPeriodDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
