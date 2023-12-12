using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Job;
using Core.Application.Features.Jobs.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.Jobs.Handlers.Queries
{
    public class DetailJobQueryHandler : IRequestHandler<DetailJobRequest, Result<JobDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailJobQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<JobDto>> Handle(DetailJobRequest request, CancellationToken cancellationToken)
        {
            var validator = new DetailBaseRequestValidator();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<JobDto>.Failure(string.Join(", ", errorMessages), (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var query = _unitOfWork.Repository<Job>().GetByIdInclude(request.id);

                if (request.isAllDetail || request.isGetTeacher)
                {
                    query = _unitOfWork.Repository<Job>().AddInclude(query, x => x.TeacherBy);
                }
                if (request.isAllDetail || request.isGetThesis)
                {
                    query = _unitOfWork.Repository<Job>().AddInclude(query, x => x.ForThesis);
                }

                var findJob = await query.SingleAsync();

                if (findJob is null)
                {
                    return Result<JobDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var JobDto = _mapper.Map<JobDto>(findJob);

                return Result<JobDto>.Success(JobDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<JobDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
