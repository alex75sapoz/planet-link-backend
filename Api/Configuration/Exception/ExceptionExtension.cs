using Microsoft.AspNetCore.Builder;

namespace Api.Configuration.Exception
{
    internal static class ExceptionExtension
    {
        public static void UseException(this IApplicationBuilder application) =>
           application.UseExceptionHandler(app => app.Run(ExceptionHandler.HandleExceptionAsync));
    }
}