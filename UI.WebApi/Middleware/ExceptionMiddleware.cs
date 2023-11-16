using Core.Application.Constants;
using Core.Application.Exceptions;
using Core.Application.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection.PortableExecutable;
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


        private int STATUS()
        {
            return 1;
        }


        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                int statusCode = 0;

                do
                {
                    var authorizationHeader = httpContext.Request.Headers["Authorization"].ToString();

                    if ((string.IsNullOrEmpty(authorizationHeader) == false && authorizationHeader.StartsWith("Bearer ")) == false)
                    {
                        break;
                    }

                    var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                    if (jwtToken == null)
                    {
                        break;
                    }

                    // Lấy danh sách quyền của người dùng từ token
                    var permissions = jwtToken.Claims
                                            .Where(c => c.Type == CONSTANT_CLAIM_TYPES.Permission)
                                            .Select(c => c.Value).ToList();

                    var endpoint = httpContext.GetEndpoint();
                    if (endpoint == null)
                    {
                        statusCode = (int)HttpStatusCode.NotImplemented;
                    }

                    var authorizeAttributes = endpoint.Metadata.GetOrderedMetadata<AuthorizeAttribute>();

                    if ((authorizeAttributes != null && authorizeAttributes.Any()) == false)
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        break;
                    }

                    var requiredRoles = authorizeAttributes
                                            .SelectMany(attr => (attr.Roles ?? "").Split(','))
                                            .Where(role => !string.IsNullOrEmpty(role))
                                            .Distinct().ToList();

                    if (requiredRoles.Except(permissions).Any())
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        break;
                    }


                } while (false);

                if (statusCode == (int)HttpStatusCode.Forbidden)
                {
                    // Người dùng không có quyền truy cập 403
                    await httpContext.Response.WriteAsync("Forbidden!");
                }
                else if (statusCode == (int)HttpStatusCode.NotImplemented)
                {
                    // Yêu cầu không thể được máy chủ thực hiện 501
                    await httpContext.Response.WriteAsync("Internal Server Error!");
                }
                else
                {
                    await _next(httpContext);
                }
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
