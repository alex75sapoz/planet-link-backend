using Api.Configuration.Authentication;
using Api.Library;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using System.Net;
using System.Security.Claims;

namespace Api.Configuration.Exception
{
    internal static class ExceptionExtension
    {
        public static void UseException(this IApplicationBuilder application) =>
           application.UseExceptionHandler(app => app.Run(async context =>
           {
               var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

               if (exception is null)
                   exception = new InternalServerException("Unknown Error");

               var statusCodeId = exception is LibraryException internalException
                                            ? (int)internalException.StatusCode
                                            : (int)HttpStatusCode.InternalServerError;

               using var scope = context.RequestServices.CreateScope();
               //var errorService = scope.ServiceProvider.GetRequiredService<ErrorService>();

               int? userSessionId = int.TryParse(context.User.FindFirstValue($"{nameof(AuthenticationResult.UserSessionId)}"), out int _userSessionId)
                                       ? _userSessionId
                                       : null;
               int? userId = int.TryParse(context.User.FindFirstValue($"{nameof(AuthenticationResult.UserId)}"), out int _userId)
                                 ? _userId
                                 : null;
               int? userTypeId = int.TryParse(context.User.FindFirstValue($"{nameof(AuthenticationResult.UserTypeId)}"), out int _userTypeId)
                                     ? _userTypeId
                                     : null;
               var timezone = (DateTimeZone)context.Items[nameof(DateTimeZone)];
               var exceptionMessage = string.Empty;

               if (exception is LibraryException libraryException)
               {
                   exceptionMessage += $"{libraryException.Message}";

                   if (libraryException.InnerException is not null)
                       exceptionMessage += $", {libraryException.InnerException.Message}";

                   if (libraryException.InternalMessage is not null)
                       exceptionMessage += $", {libraryException.InternalMessage}";
               }
               else
               {
                   exceptionMessage += $"{exception.Message}";

                   if (exception.InnerException is not null)
                       exceptionMessage += $", {exception.InnerException.Message}";
               }

               //await errorService.CreateErrorRequestAsync(new ErrorRequestCreateContract()
               //{
               //    Method = context.Request.Method,
               //    Path = context.Request.Path,
               //    Query = context.Request.QueryString.ToString(),
               //    StatusCodeId = statusCodeId,
               //    ExceptionType = exception.GetType().Name,
               //    ExceptionMessage = exceptionMessage,
               //    ExceptionStackTrace = exception.StackTrace,
               //    UserSessionId = userTypeId == (int)UserType.Guest
               //                        ? null
               //                        : userSessionId,
               //    UserId = userTypeId == (int)UserType.Guest
               //                 ? null
               //                 : userId,
               //    TimezoneId = timezone.Id
               //});

               context.Response.StatusCode = statusCodeId;
               await context.Response.WriteAsJsonAsync(exception.InnerException is not null
                       ? $"{exception.Message}, {exception.InnerException}"
                       : $"{exception.Message}"
               );
           }));
    }
}