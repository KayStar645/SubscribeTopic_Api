﻿using Core.Application.DTOs.StudentJoin;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.StudentJoins.Requests.Commands;
using Core.Application.Features.StudentJoins.Requests.Queries;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UI.WebApi.Controllers
{
    [Route("api/studentJoin")]
    [ApiController]
    public class StudentJoinController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentJoinController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<StudentJoinDto>>> Get([FromQuery] ListStudentJoinRequest<StudentJoinDto> request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        [HttpGet("detail")]
        public async Task<ActionResult<StudentJoinDto>> Get([FromQuery] DetailStudentJoinRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        [HttpPost]
        public async Task<ActionResult<StudentJoinDto>> Post([FromBody] CreateStudentJoinDto StudentJoinRequest)
        {
            var command = new CreateStudentJoinRequest { createStudentJoinDto = StudentJoinRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UpdateStudentJoinDto StudentJoinRequest)
        {
            var command = new UpdateStudentJoinRequest { updateStudentJoinDto = StudentJoinRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromForm] DeleteBaseRequest<StudentJoin> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new { Error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Error = ResponseTranform.ServerError });
            }
        }
    }
}