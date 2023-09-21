using AutoMapper;
using Core.Application.DTOs.RegistrationPeriod;
using Core.Application.Features.RegistrationPeriods.Requests.Queries;
using Core.Application.Interfaces.Repositories;
using Core.Application.Responses;
using Core.Application.Transform;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.RegistrationPeriods.Handlers.Queries
{
    public class DetailRegistrationPeriodRequestHandler : IRequestHandler<DetailRegistrationPeriodRequest, Result<RegistrationPeriodDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRegistrationPeriodRepository _registrationPeriodRepo;

        public DetailRegistrationPeriodRequestHandler(IRegistrationPeriodRepository registrationPeriodRepository, IMapper mapper)
        {
            _mapper = mapper;
            _registrationPeriodRepo = registrationPeriodRepository;
        }

        public async Task<Result<RegistrationPeriodDto>> Handle(DetailRegistrationPeriodRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _registrationPeriodRepo.GetByIdInclude(request.id);

                if (request.isAllDetail)
                {
                    query = _registrationPeriodRepo.AddInclude(query, x => x.Faculty);
                }
                else
                {
                    if (request.isGetFaculty == true)
                    {
                        query = _registrationPeriodRepo.AddInclude(query, x => x.Faculty);
                    }
                }

                var findPeriod = await query.SingleAsync();

                if (findPeriod is null)
                {
                    return Result<RegistrationPeriodDto>.Failure(
                        ValidatorTranform.NotExistsValue("Id", request.id.ToString()),
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
