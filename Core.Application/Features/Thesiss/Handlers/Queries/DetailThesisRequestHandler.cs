using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Group;
using Core.Application.DTOs.Major;
using Core.Application.DTOs.StudentJoin;
using Core.Application.DTOs.Teacher;
using Core.Application.DTOs.Thesis;
using Core.Application.Features.Thesiss.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography;

namespace Core.Application.Features.Thesiss.Handlers.Queries
{
    public class DetailThesisRequestHandler : IRequestHandler<DetailThesisRequest, Result<ThesisDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailThesisRequestHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ThesisDto>> Handle(DetailThesisRequest request, CancellationToken cancellationToken)
        {
            var validator = new DetailBaseRequestValidator();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<ThesisDto>.Failure(string.Join(", ", errorMessages), (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var query = _unitOfWork.Repository<Thesis>().GetByIdInclude(request.id);

                var findThesis = await query.SingleAsync();

                if (findThesis is null)
                {
                    return Result<ThesisDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var thesisDto = _mapper.Map<ThesisDto>(findThesis);

                if (request.isAllDetail == true || request.isGetThesisInstructions == true)
                {
                    var thesisInstructions = await _unitOfWork.Repository<ThesisInstruction>()
                                                .Query()
                                                .Where(x => x.ThesisId == findThesis.Id)
                                                .Include(x => x.Teacher)
                                                .Select(x => x.Teacher)
                                                .ToListAsync();

                    thesisDto.ThesisInstructions = _mapper.Map<List<TeacherDto>>(thesisInstructions);
                }

                if (request.isAllDetail == true || request.isGetThesisReviews == true)
                {
                    var thesisReviews = await _unitOfWork.Repository<ThesisReview>()
                                                .Query()
                                                .Where(x => x.ThesisId == findThesis.Id)
                                                .Include(x => x.Teacher)
                                                .Select(x => x.Teacher)
                                                .ToListAsync();

                    thesisDto.ThesisReviews = _mapper.Map<List<TeacherDto>>(thesisReviews);
                }

                if (request.isAllDetail == true || request.isGetThesisMajors == true)
                {
                    var thesisMajors = await _unitOfWork.Repository<ThesisMajor>()
                                                .Query()
                                                .Where(x => x.ThesisId == findThesis.Id)
                                                .Include(x => x.Major)
                                                .Select(x => x.Major)
                                                .ToListAsync();

                    thesisDto.ThesisMajors = _mapper.Map<List<MajorDto>>(thesisMajors);
                }

                return Result<ThesisDto>.Success(thesisDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
