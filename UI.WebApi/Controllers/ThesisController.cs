using Core.Application.DTOs.Thesis;
using Core.Application.DTOs.Thesis.Validators;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Thesiss.Requests.Commands;
using Core.Application.Features.Thesiss.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/Thesis")]
    [ApiController]
    public class ThesisController : Controller
    {
        private readonly IMediator _mediator;

        public ThesisController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách đề tài theo ngành/khoa
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// facultyId/industryId: required
        /// 
        /// </remarks>
        [HttpGet]
        [Permission("Thesis.View")]
        public async Task<ActionResult> Get([FromQuery] ListThesisRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy thông tin đề tài theo mã
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpGet("detail")]
        [Permission("Thesis.View")]
        public async Task<ActionResult> Get([FromQuery] DetailThesisRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Thêm đề tài
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Name: string, required, max(190), unique
        /// - internalCode: string, required, max(190), unique
        /// </remarks>
        [HttpPost]
        [Permission("Thesis.Create")]
        public async Task<ActionResult> Post([FromBody] CreateThesisDto request)
        {
            var command = new CreateThesisRequest { createThesisDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Sửa đề tài
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// - Name: string, required, max(190), unique
        /// - internalCode: string, required, max(190), unique
        /// </remarks>
        [HttpPut]
        [Permission("Thesis.Update")]
        public async Task<ActionResult> Put([FromBody] UpdateThesisDto request)
        {
            var command = new UpdateThesisRequest { updateThesisDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Xóa đề tài
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpDelete]
        [Permission("Thesis.Delete")]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<Thesis> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                var responses = Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                return StatusCode(responses.Code, responses);
            }
            catch (BadRequestException ex)
            {
                var responses = Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                return StatusCode(responses.Code, responses);
            }
            catch (Exception ex)
            {
                var responses = Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                return StatusCode(responses.Code, responses);
            }
        }

        /// <summary>
        /// Yêu cầu duyệt/Hủy yêu cầu/Yêu cầu chỉnh sửa lại
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// - Status: Required
        /// D -> AR
        /// AR -> A/D
        /// 
        /// </remarks>
        [HttpPut("ChangeStatus")]
        [Permission("Thesis.Change")]
        public async Task<ActionResult> ChangeStatus([FromBody] ChangeStatusThesisDto request)
        {
            var command = new ChangeStatusThesisRequest { changeStatusThesis = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Duyệt đề tài
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpPut("Approve")]
        [Permission("Thesis.Approve")]
        public async Task<ActionResult> Approve([FromBody] ApproveThesisDto request)
        {
            var command = new ApproveThesisRequest { approveThesisDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Sinh viên lấy danh sách đề tài
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// -
        /// </remarks>
        [HttpGet("ListThesisRegistration")]
        [Permission("Thesis.Student.View")]
        public async Task<ActionResult> Get([FromQuery] ListThesisRegistrationRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Giảng viên lấy ds đề tài mình hướng dẫn
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// -
        /// </remarks>
        [HttpGet("ListThesisInstructorOfTeacher")]
        [Permission("Thesis.InstructorOfTeacher.View")]
        public async Task<ActionResult> Get([FromQuery] ListThesisInstructorOfTeacherRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Giảng viên lấy ds đề tài mình phản biện
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// -
        /// </remarks>
        [HttpGet("ListThesisReviewOfTeacher")]
        [Permission("Thesis.InstructorOfTeacher.View")]
        public async Task<ActionResult> Get([FromQuery] ListThesisReviewOfTeacherRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy danh sách đề tài có thể đưa ra hội đồng
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet("ListThesisPossibleCouncil")]
        [Permission("Thesis.PossibleCouncil.View")]
        public async Task<ActionResult> Get([FromQuery] ListThesisPossibleCouncilRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Đưa đề tài ra hội đồng
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - ThesisId: int, required
        /// </remarks>
        [HttpPut("ApproveToCouncil")]
        [Permission("Thesis.ApproveToCouncil")]
        public async Task<ActionResult> ApproveToCouncil([FromBody] ApproveThesisToCouncilRequest request)
        {
            var command = request;
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }
    }
}
