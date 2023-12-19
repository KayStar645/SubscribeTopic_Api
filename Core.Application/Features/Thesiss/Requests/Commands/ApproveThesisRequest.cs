using Core.Application.DTOs.Thesis;
using Core.Application.DTOs.Thesis.Validators;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Thesiss.Requests.Commands
{
    public class ApproveThesisRequest : IRequest<Result<ThesisDto>>
    {
        public ApproveThesisDto? approveThesisDto { get; set; }
    }
}
