﻿using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.Features.Thesiss.Events
{
    public class BeforeCreateThesisUpdateThesisEvent : INotification
    {
        public Thesis _thesis { get; set; }

        public IHttpContextAccessor _httpContextAccessor;
        public IUnitOfWork _unitOfWork;

        public BeforeCreateThesisUpdateThesisEvent(Thesis thesis, 
            IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _thesis = thesis;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
    }

    public class BeforeCreateThesisUpdateThesisHandler : INotificationHandler<BeforeCreateThesisUpdateThesisEvent>
    {
        public async Task Handle(BeforeCreateThesisUpdateThesisEvent pEvent, CancellationToken cancellationToken)
        {
            try
            {
                var userId = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
                var userType = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;

                if(userType == CLAIMS_VALUES.TYPE_TEACHER)
                {
                    // Từ id của người dùng lấy ra id của giáo viên
                    var teacher = await pEvent._unitOfWork.Repository<Teacher>()
                        .FirstOrDefaultAsync(x => x.UserId == int.Parse(userId));

                    pEvent._thesis.LecturerThesisId = teacher.Id;
                    //pEvent._thesis.LecturerThesis = teacher;
                    pEvent._thesis.Type = Thesis.TYPE_LECTURER_OUT;
                }    
                else if(userType == CLAIMS_VALUES.TYPE_STUDENT)
                {
                    var student = await pEvent._unitOfWork.Repository<Student>()
                        .FirstOrDefaultAsync(x => x.UserId == int.Parse(userId));

                    pEvent._thesis.ProposedStudentId = student.Id;
                    //pEvent._thesis.ProposedStudent = student;
                    pEvent._thesis.Type = Thesis.TYPE_STUDENT_PROPOSAL;
                }
                pEvent._thesis.Status = Thesis.STATUS_DRAFT;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }

}
