using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Group;
using Core.Application.DTOs.Major;
using Core.Application.DTOs.StudentJoin;
using Core.Application.DTOs.Teacher;
using Core.Application.DTOs.Thesis;
using Core.Application.DTOs.ThesisRegistration;
using Core.Application.Features.ThesisRegistrations.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography;

namespace Core.Application.Features.ThesisRegistrations.Handlers.Queries
{
    public class DetailThesisRegistrationRequestHandler : IRequestHandler<DetailThesisRegistrationRequest, Result<ThesisRegistrationDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailThesisRegistrationRequestHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ThesisRegistrationDto>> Handle(DetailThesisRegistrationRequest request, CancellationToken cancellationToken)
        {
            var validator = new DetailBaseRequestValidator();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<ThesisRegistrationDto>.Failure(string.Join(", ", errorMessages), (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var query = _unitOfWork.Repository<ThesisRegistration>().GetByIdInclude(request.id);

                if (request.isAllDetail || request.isGetThesis == true)
                {
                    query = query.Include(x => x.Thesis);
                }

                if (request.isAllDetail || request.isGetGroup == true)
                {
                    query = query.Include(x => x.Group);
                }

                var findThesisRegistration = await query.SingleAsync();

                if (findThesisRegistration is null)
                {
                    return Result<ThesisRegistrationDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var thesisRegistrationDto = _mapper.Map<ThesisRegistrationDto>(findThesisRegistration);

                // Detail infor Thesis
                if (request.isAllDetail == true || request.isGetThesis == true)
                {
                    var thesisInstructions = await _unitOfWork.Repository<ThesisInstruction>()
                                                .Query()
                                                .Where(x => x.ThesisId == thesisRegistrationDto.ThesisId)
                                                .Include(x => x.Teacher)
                    .Select(x => x.Teacher)
                                                .ToListAsync();

                    thesisRegistrationDto.Thesis.ThesisInstructions = _mapper.Map<List<TeacherDto>>(thesisInstructions);

                    var thesisReviews = await _unitOfWork.Repository<ThesisReview>()
                                                .Query()
                                                .Where(x => x.ThesisId == thesisRegistrationDto.ThesisId)
                                                .Include(x => x.Teacher)
                                                .Select(x => x.Teacher)
                    .ToListAsync();

                    thesisRegistrationDto.Thesis.ThesisReviews = _mapper.Map<List<TeacherDto>>(thesisReviews);

                    var thesisMajors = await _unitOfWork.Repository<ThesisMajor>()
                                                .Query()
                                                .Where(x => x.ThesisId == thesisRegistrationDto.ThesisId)
                                                .Include(x => x.Major)
                                                .Select(x => x.Major)
                    .ToListAsync();

                    thesisRegistrationDto.Thesis.ThesisMajors = _mapper.Map<List<MajorDto>>(thesisMajors);
                }

                // Detail infor Group
                if(request.isAllDetail == true || request.isGetGroup == true)
                {
                    var members = await _unitOfWork.Repository<StudentJoin>()
                                        .Query()
                                        .Where(x => x.GroupId == thesisRegistrationDto.GroupId)
                    .Include(x => x.Student)
                                        .ToListAsync();

                    thesisRegistrationDto.Group.Members = _mapper.Map<List<StudentJoinDto>>(members);
                }    

                return Result<ThesisRegistrationDto>.Success(thesisRegistrationDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<ThesisRegistrationDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}