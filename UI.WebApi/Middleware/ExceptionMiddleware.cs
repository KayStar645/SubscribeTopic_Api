using Azure.Core;
using Core.Application.Constants;
using Core.Application.Exceptions;
using Core.Application.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace API.WebApi.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                var authorizationHeader = httpContext.Request.Headers["Authorization"].ToString();

                authorizationHeader = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1aWQiOiIyIiwidXNlck5hbWUiOiJHVjAwMDAxIiwidHlwZSI6InRlYWNoZXIiLCJmYWN1bHR5Ijoie1wiSW50ZXJuYWxDb2RlXCI6XCJDTlRUXCIsXCJOYW1lXCI6XCJDXFx1MDBGNG5nIG5naFxcdTFFQzcgdGhcXHUwMEY0bmcgdGluXCIsXCJBZGRyZXNzXCI6XCJCMS4wMlwiLFwiUGhvbmVOdW1iZXJcIjpcIjAzMjE1NDY1ODdcIixcIkVtYWlsXCI6XCJjbnR0QGh1ZmkuZWR1LnZuXCIsXCJEZWFuX1RlYWNoZXJJZFwiOm51bGwsXCJEZWFuX1RlYWNoZXJcIjpudWxsLFwiSWRcIjoyfSIsImV4cCI6MTcwMDA2OTI3OSwiaXNzIjoiU3Vic2NyaWJlVG9waWMiLCJhdWQiOiJTdWJzY3JpYmVUb3BpY1VzZXIifQ.6Dv8luXLPCzgWmIwmdBlobFqVM0ji82D4Gr6Gmu_R8I";

                if (string.IsNullOrEmpty(authorizationHeader) == false && authorizationHeader.StartsWith("Bearer "))
                {
                    var token = authorizationHeader.Substring("Bearer ".Length).Trim();

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                    if (jwtToken != null)
                    {
                        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == CONSTANT_CLAIM_TYPES.Uid)?.Value;
                        var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == CONSTANT_CLAIM_TYPES.UserName)?.Value;

                        var requestPath = httpContext.Request?.Path.Value ?? "";
                        var controller = requestPath.Substring(requestPath.LastIndexOf('/') + 1);
                        var requestMethod = httpContext.Request?.Method ?? "";

                        var permission = requestMethod + "_" + controller;


                        var isAuthenticated = !string.IsNullOrEmpty(userId);
                        // Lấy danh sách quyền
                        var hasPermission = true;// Kiểm tra quyền của người dùng dựa trên các claim khác trong token.

                        var endpoint = httpContext.GetEndpoint();
                        if (endpoint?.Metadata != null)
                        {
                            var controllerActionDescriptor = endpoint.Metadata.OfType<ControllerActionDescriptor>().FirstOrDefault();
                            if (controllerActionDescriptor != null)
                            {
                                var controllerName = controllerActionDescriptor.ControllerName;
                                var actionName = controllerActionDescriptor.ActionName;

                                // Lấy quyền của action hiện tại
                                var requiredRoles = controllerActionDescriptor.EndpointMetadata
                                    .OfType<AuthorizeAttribute>()
                                    .Select(attr => attr.Roles)
                                    .FirstOrDefault();

                                // ...
                            }
                        }

                        var roles = "";
                        // Kiểm tra quyền
                        if (roles.Contains("ViewFaculty"))
                        {
                            // Có quyền truy cập vào action
                        }
                        else
                        {
                            // Không có quyền, xử lý tương ứng
                            //return StatusCode(403); // Hoặc một mã lỗi tương ứng với việc không có quyền truy cập
                        }

                        if (isAuthenticated && hasPermission)
                        {
                            httpContext.Items["UserId"] = userId;
                            httpContext.Items["UserName"] = userName;

                            // Lấy thông tin về controller và action hiện tại
                            //var endpoint = httpContext.GetEndpoint();
                            //if (endpoint?.Metadata != null)
                            //{
                            //    var controllerActionDescriptor = endpoint.Metadata.OfType<ControllerActionDescriptor>().FirstOrDefault();
                            //    if (controllerActionDescriptor != null)
                            //    {
                            //        var controllerName = controllerActionDescriptor.ControllerName;
                            //        var actionName = controllerActionDescriptor.ActionName;

                            //        // Lấy quyền của action hiện tại
                            //        var requiredRoles = controllerActionDescriptor.EndpointMetadata
                            //            .OfType<AuthorizeAttribute>()
                            //            .Select(attr => attr.Roles)
                            //            .FirstOrDefault();

                            //        // ...
                            //    }
                            //}

                        }
                        else
                        {
                            // Người dùng không có quyền, xử lý tương ứng
                            // Ví dụ: throw new ForbiddenException("Access denied");
                        }
                    }
                }

                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private bool IsUserAuthorized(ClaimsPrincipal user)
        {
            return user.IsInRole("Admin");
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string result = JsonConvert.SerializeObject(new ErrorDeatils
            {
                ErrorMessage = exception.Message,
                ErrorType = ResponseTranform.Fail
            });

            switch (exception)
            {
                case BadRequestException badRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case ValidationException validationException:
                    statusCode = HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(validationException.Errors);
                    break;
                case NotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    break;
                default:
                    break;
            }

            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(result);
        }
    }

    public class ErrorDeatils
    {
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
    }
}
