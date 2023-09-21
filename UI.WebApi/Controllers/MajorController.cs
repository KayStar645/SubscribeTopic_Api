﻿using Core.Application.DTOs.Major;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Majors.Requests.Commands;
using Core.Application.Features.Majors.Requests.Queries;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UI.WebApi.Controllers
{
    [Route("api/major")]
    [ApiController]
    [Authorize]
    public class MajorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MajorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<MajorDto>>> Get([FromQuery] ListMajorRequest<MajorDto> request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        [HttpGet("detail")]
        public async Task<ActionResult<MajorDto>> Get([FromQuery] DetailMajorRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        [HttpPost]
        public async Task<ActionResult<MajorDto>> Post([FromBody] CreateMajorDto majorRequest)
        {
            var command = new CreateMajorRequest { createMajorDto = majorRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }
        
        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UpdateMajorDto majorRequest)
        {
            var command = new UpdateMajorRequest { updateMajorDto = majorRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromForm] DeleteBaseRequest<Major> request)
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
