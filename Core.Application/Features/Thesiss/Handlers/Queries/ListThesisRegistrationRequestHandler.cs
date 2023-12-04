using AutoMapper;
using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Major;
using Core.Application.DTOs.Teacher;
using Core.Application.DTOs.Thesis;
using Core.Application.Exceptions;
using Core.Application.Features.Thesiss.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Thesiss.Handlers.Queries
{
    public class ListThesisRegistrationRequestHandler : IRequestHandler<ListThesisRegistrationRequest, PaginatedResult<List<ThesisDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        public IHttpContextAccessor _httpContextAccessor;

        public ListThesisRegistrationRequestHandler(IUnitOfWork unitOfWork, IMapper mapper,
            ISieveProcessor sieveProcessor, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
            _httpContextAccessor = httpContextAccessor;
        }

        // Hiển thị 2 dạng
        // + Tất cả đề tài trong đợt đăng ký
        // + Những đề tài mà nhóm có thể đăng ký
        public async Task<PaginatedResult<List<ThesisDto>>> Handle(ListThesisRegistrationRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var sieve = _mapper.Map<SieveModel>(request);

                var query = _unitOfWork.Repository<Thesis>().GetAllInclude();                

                var facultyId = _httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.FacultyId)?.Value;
                if(facultyId == null)
                {
                    throw new UnauthorizedAccessException(IdentityTranform.ForbiddenException());
                }    


                // Lấy đợt đăng ký hiện tại
                var period = await _unitOfWork.Repository<RegistrationPeriod>()
                                              .Query()
                                              .Where(x => x.FacultyId == int.Parse(facultyId) && x.IsActive && 
                                                     x.TimeStart <= DateTime.Now && DateTime.Now <= x.TimeEnd)
                                              .FirstOrDefaultAsync();
                if (period == null)
                {
                    throw new UnauthorizedAccessException("Chưa tới thời gian đăng ký đề tài!");
                }
                query = query.Where(x => x.Status == Thesis.STATUS_APPROVED);

                // Kiểm tra số lượng thành viên nhóm

                // Kiểm tra chuyên ngành phù hợp


                if (request.isAllDetail == true || request.isGetIssuer == true)
                {
                    query = _unitOfWork.Repository<Thesis>().AddInclude(query, x => x.LecturerThesis);
                }

                query = _sieveProcessor.Apply(sieve, query);

                int totalCount = await query.CountAsync();

                var thesiss = await query.ToListAsync();

                var mapThesiss = _mapper.Map<List<ThesisDto>>(thesiss);

                foreach (var thesis in mapThesiss)
                {
                    if (request.isAllDetail == true || request.isGetThesisInstructions == true)
                    {
                        var thesisInstructions = await _unitOfWork.Repository<ThesisInstruction>()
                                                    .Query()
                                                    .Where(x => x.ThesisId == thesis.Id)
                                                    .Include(x => x.Teacher)
                                                    .Select(x => x.Teacher)
                                                    .ToListAsync();

                        thesis.ThesisInstructions = _mapper.Map<List<TeacherDto>>(thesisInstructions);
                    }

                    if (request.isAllDetail == true || request.isGetThesisReviews == true)
                    {
                        var thesisReviews = await _unitOfWork.Repository<ThesisReview>()
                                                    .Query()
                                                    .Where(x => x.ThesisId == thesis.Id)
                                                    .Include(x => x.Teacher)
                                                    .Select(x => x.Teacher)
                                                    .ToListAsync();

                        thesis.ThesisReviews = _mapper.Map<List<TeacherDto>>(thesisReviews);
                    }

                    if (request.isAllDetail == true || request.isGetThesisMajors == true)
                    {
                        var thesisMajors = await _unitOfWork.Repository<ThesisMajor>()
                                                    .Query()
                                                    .Where(x => x.ThesisId == thesis.Id)
                                                    .Include(x => x.Major)
                                                    .Select(x => x.Major)
                                                    .ToListAsync();

                        thesis.ThesisMajors = _mapper.Map<List<MajorDto>>(thesisMajors);
                    }
                }

                return PaginatedResult<List<ThesisDto>>.Success(
                    mapThesiss, totalCount, request.page,
                    request.pageSize);
            }

            catch (NotFoundException ex)
            {
                return PaginatedResult<List<ThesisDto>>.Failure((int)HttpStatusCode.NotFound, new List<string> { ex.Message });
            }
            catch (BadRequestException ex)
            {
                return PaginatedResult<List<ThesisDto>>.Failure((int)HttpStatusCode.BadRequest, new List<string> { ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return PaginatedResult<List<ThesisDto>>.Failure(ex.ErrorCode, new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                return PaginatedResult<List<ThesisDto>>.Failure((int)HttpStatusCode.InternalServerError, new List<string> { ex.Message });
            }
        }
    }
}
