using Library.Application;
using Library.Application.Contract;
using Library.Base;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Threading.Tasks;

namespace Api.Configuration.Exception
{
    static class ExceptionHandler
    {
        public static async Task HandleExceptionAsync(HttpContext context)
        {
            var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error ?? new InternalServerException("Unknown Error");

            string exceptionMessage;
            int statusCodeId;

            if (exception is BaseException baseException)
            {
                statusCodeId = (int)baseException.StatusCode;
                exceptionMessage = baseException.GetFullMessage();

                if (baseException.InternalMessage is not null)
                    exceptionMessage += $", {baseException.InternalMessage}";
            }
            else
            {
                statusCodeId = (int)HttpStatusCode.InternalServerError;
                exceptionMessage = exception.GetFullMessage();
            }

            using var scope = context.RequestServices.CreateScope();
            var applicationService = scope.ServiceProvider.GetRequiredService<IApplicationService>();

            var authenticationResult = new AuthenticationResult(context.User);

            await applicationService.CreateErrorRequestAsync(new ApplicationErrorRequestCreateContract
            (
                method: context.Request.Method,
                path: context.Request.Path,
                query: context.Request.QueryString.ToString(),
                statusCodeId: statusCodeId,
                exceptionType: exception.GetType().Name,
                exceptionMessage: exceptionMessage,
                timezoneId: authenticationResult.Timezone.Id
            )
            {
                UserSessionId = authenticationResult.UserSessionId,
                UserId = authenticationResult.UserId
            });

            context.Response.StatusCode = statusCodeId;
            await context.Response.WriteAsJsonAsync(exception.InnerException is not null
                    ? $"{exception.Message}, {exception.InnerException}"
                    : $"{exception.Message}"
            );
        }
    }
}