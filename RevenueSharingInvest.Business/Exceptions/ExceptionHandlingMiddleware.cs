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
    public class ExceptionHandlingMiddleware
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

            }

            var errorMessage = JsonConvert.SerializeObject(errorMessageObject);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(errorMessage);
        }

    }
}
