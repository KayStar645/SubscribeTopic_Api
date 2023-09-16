﻿using Core.Application.Features.Base.Requests.Queries;
using Core.Domain.Entities;

namespace Core.Application.Features.Teachers.Requests.Queries
{
    public class ListDepartmentRequest<T> : ListBaseRequest<T>
    {
        public bool IsGetDepartment { get; set; }
        public string? Type { get; set; }
    }
}
