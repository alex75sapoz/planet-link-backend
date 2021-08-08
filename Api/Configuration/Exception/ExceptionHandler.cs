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

            context.Response.StatusCode = exception is BaseException baseException
                ? (int)baseException.StatusCode
                : (int)HttpStatusCode.InternalServerError;

            using var scope = context.RequestServices.CreateScope();
            var applicationService = scope.ServiceProvider.GetRequiredService<IApplicationService>();

            var authenticationResult = new AuthenticationResult(context.User);

            await applicationService.CreateErrorRequestAsync(new ApplicationErrorRequestCreateContract
            (
                method: context.Request.Method,
                path: context.Request.Path,
                query: context.Request.QueryString.ToString(),
                statusCodeId: context.Response.StatusCode,
                exceptionType: exception.GetType().Name,
                exceptionMessage: exception.GetFullMessage(),
                timezoneId: authenticationResult.Timezone.Id
            )
            {
                UserSessionId = authenticationResult.UserSessionId,
                UserId = authenticationResult.UserId
            });

            await context.Response.WriteAsJsonAsync(exception.GetFullMessage(includeInternalMessage: false));
        }
    }
}