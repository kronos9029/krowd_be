using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RevenueSharingInvest.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Exceptions
{
    public class ExceptionHandlingMiddleware : AuthorizeAttribute
    {
        public RequestDelegate requestDelegate;
        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate)
        {
            this.requestDelegate = requestDelegate;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await requestDelegate(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private static Task HandleException(HttpContext context, Exception ex)
        {
            var errorMessageObject = new ErrorResponse { Message = ex.Message, Code = "500" };
            var statusCode = (int)HttpStatusCode.InternalServerError;
            switch (ex)
            {
                case UnauthorizedException:
                    errorMessageObject.Code = "401";
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                case RegisterException:
                    errorMessageObject.Code = "R001";
                    statusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case UnauthorizedAccessException:
                    errorMessageObject.Code = "U001";
                    statusCode = (int)HttpStatusCode.ServiceUnavailable;
                    break;
                case NotFoundException:
                    errorMessageObject.Code = "N001";
                    statusCode = (int)HttpStatusCode.NotFound;
                    break;
                case CreateBusinessException:
                    errorMessageObject.Code = "B001";
                    statusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case FileException:
                    errorMessageObject.Code = "F001";
                    statusCode = (int)HttpStatusCode.Conflict;
                    break;
                case InUseException:
                    errorMessageObject.Code = "I001";
                    statusCode = (int)HttpStatusCode.Conflict;
                    break;
                case WalletBalanceException:
                    errorMessageObject.Code = "W001";
                    statusCode = (int)HttpStatusCode.Conflict;
                    break ;
                case CreateProjectEntityException:
                    errorMessageObject.Code = "PE001";
                    statusCode = (int)HttpStatusCode.Ambiguous;
                    break;
                case LoginException:
                    errorMessageObject.Code = "L001";
                    statusCode= (int)HttpStatusCode.BadRequest;
                    break;
                case AmountExcessException:
                    errorMessageObject.Code = "A001";
                    statusCode=(int)HttpStatusCode.BadRequest;
                    break;
                case FormatException:
                    errorMessageObject.Code = "F002";
                    statusCode = (int)HttpStatusCode.BadRequest;
                    break;

            }

            var errorMessage = JsonConvert.SerializeObject(errorMessageObject);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(errorMessage);
        }

    }
}
