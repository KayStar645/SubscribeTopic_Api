using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Feedback;
using Core.Application.DTOs.Group;
using Core.Application.Features.Feedbacks.Requests.Queries;
using Core.Application.Features.Groups.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Feedbacks.Handlers.Queries
{
    public class ListFeedbackRequestHandler : IRequestHandler<ListFeedbackRequest, PaginatedResult<List<FeedbackDto>>>
    {
        readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;

        public ListFeedbackRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<PaginatedResult<List<FeedbackDto>>> Handle(ListFeedbackRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListFeedbackDtoValidator(_unitOfWork);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<FeedbackDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            // Start Query
            var query = _unitOfWork.Repository<Feedback>().GetAllInclude();

            query = query.Where(x => x.ThesisId == request.thesisId);

            query = _unitOfWork.Repository<Feedback>().AddInclude(query, x => x.Commenter);
            // End Query

            query = _sieveProcessor.Apply(sieve, query);

            int totalCount = await query.CountAsync();

            var groups = await query.ToListAsync();

            var mapGroups = _mapper.Map<List<FeedbackDto>>(groups);
            return PaginatedResult<List<FeedbackDto>>.Success(
                mapGroups, totalCount, request.page,
                request.pageSize);
        }
    }
}
