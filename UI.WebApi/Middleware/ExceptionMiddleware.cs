using Core.Application.Constants;
using Core.Application.Exceptions;
using Core.Application.Transform;
using Microsoft.AspNetCore.Authorization;
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

                if (string.IsNullOrEmpty(authorizationHeader) == false && authorizationHeader.StartsWith("Bearer "))
                {
                    var token = authorizationHeader.Substring("Bearer ".Length).Trim();

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                    if (jwtToken != null)
                    {
                        // Lấy danh sách quyền của người dùng từ token
                        var permissions = jwtToken.Claims
                                                .Where(c => c.Type == CONSTANT_CLAIM_TYPES.Permission)
                                                .Select(c => c.Value).ToList();

                        // Lấy danh sách quyền yêu cầu của action hiện tại
                        var endpoint = httpContext.GetEndpoint();
                        var authorizationAttributes = endpoint?.Metadata.GetOrderedMetadata<IAuthorizeData>();
                        var requiredRoles = authorizationAttributes?.SelectMany(attr => attr.Roles.Split(','));

                        // Kiểm tra xem người dùng có quyền truy cập hay không
                        //if (requiredRoles != null && requiredRoles.Any() && userRoles.Intersect(requiredRoles).Any())
                        //{
                        //    // Người dùng có quyền truy cập
                        //}
                        //else
                        //{
                        //    // Người dùng không có quyền truy cập
                        //    httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        //    await httpContext.Response.WriteAsync("Access Denied");
                        //    return;
                        //}
                    }
                }

                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
                httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await httpContext.Response.WriteAsync("Access Denied: " + ex.Message);
                return;
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
