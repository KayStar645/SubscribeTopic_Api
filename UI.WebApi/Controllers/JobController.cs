using Core.Application.DTOs.Job;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Jobs.Requests.Commands;
using Core.Application.Features.Jobs.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/job")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JobController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách công việc theo đề tài
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// thesisId: required
        /// 
        /// </remarks>
        [HttpGet]
        [Permission("Job.View")]
        public async Task<ActionResult> Get([FromQuery] ListJobRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy thông tin công việc
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpGet("detail")]
        [Permission("Job.View")]
        public async Task<ActionResult> Get([FromQuery] DetailJobRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// GV thêm công việc cho đề tài
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Name: string, required, max(190)
        /// - Instructions: string, ckeditor
        /// - Due: Thời gian phải lớn hơn hiện tại
        /// - Files: Danh sách file kèm theo (nếu có)
        /// - ThesisId: Id của đề tài
        /// </remarks>
        [HttpPost]
        [Permission("Job.Create")]
        public async Task<ActionResult> Post([FromBody] CreateJobDto request)
        {
            var command = new CreateJobRequest { createJobDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Sửa công việc
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// - Name: string, required, max(190)
        /// - Instructions: string, ckeditor
        /// - Due: Thời gian phải lớn hơn hiện tại
        /// - Files: Danh sách file kèm theo (nếu có)
        /// - ThesisId: Id của đề tài
        /// </remarks>
        [HttpPut]
        [Permission("Job.Update")]
        public async Task<ActionResult> Put([FromBody] UpdateJobDto request)
        {
            var command = new UpdateJobRequest { updateJobDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Xóa Công việc
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpDelete]
        [Permission("Job.Delete")]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<Job> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                var responses = Result<JobDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                return StatusCode(responses.Code, responses);
            }
            catch (BadRequestException ex)
            {
                var responses = Result<JobDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                return StatusCode(responses.Code, responses);
            }
            catch (Exception ex)
            {
                var responses = Result<JobDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                return StatusCode(responses.Code, responses);
            }
        }
    }
}
