using NodaTime;
using RestSharp;
using System;
using System.Net;

namespace Api.Library
{
    public static class LibraryExtension
    {
        #region Noda Time

        public static readonly DateTimeZone EasternTimezone = GetTimezone("America/New_York");

        public static DateTimeZone GetTimezone(string timezoneId) =>
            DateTimeZoneProviders.Tzdb.GetZoneOrNull(timezoneId) ?? throw new BadRequestException($"{nameof(timezoneId)} is invalid");

        public static DateTimeOffset AtTimezone(this DateTimeOffset input, DateTimeZone timezone) =>
            new ZonedDateTime(Instant.FromDateTimeOffset(input), timezone).ToDateTimeOffset();

        public static DateTimeOffset AtTimezone(this DateTimeOffset input, string timezoneId) =>
            input.AtTimezone(GetTimezone(timezoneId));

        public static DateTimeOffset SetTimezone(this DateTime input, DateTimeZone timezone) =>
            LocalDateTime.FromDateTime(input).InZoneLeniently(timezone).ToDateTimeOffset();

        public static DateTimeOffset SetTimezone(this DateTime input, string timezoneId) =>
            input.SetTimezone(GetTimezone(timezoneId));

        #endregion

        #region RestSharp

        public static T GetData<T>(this IRestResponse<T> response, Func<T, bool> isSuccess)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var internalMessage = string.Empty;

                internalMessage += $"Method: {response.Request.Method}";
                internalMessage += $", Request: {response.Request.Resource}";
                internalMessage += $", Response: {response.Content}";
                internalMessage += $", StatusCode: {response.StatusCode}";
                if (response.ErrorMessage is not null)
                    internalMessage += $", {response.ErrorMessage}";

                throw new BadRequestException($"Data requested from an external server was not successful", internalMessage);
            }
            else if (isSuccess is not null && !isSuccess.Invoke(response.Data))
            {
                var internalMessage = string.Empty;

                internalMessage += $"Method: {response.Request.Method}";
                internalMessage += $", Request: {response.Request.Resource}";
                internalMessage += $", Response: {response.Content}";
                internalMessage += $", StatusCode: {response.StatusCode}";
                if (response.ErrorMessage is not null)
                    internalMessage += $", {response.ErrorMessage}";

                throw new BadRequestException($"Data requested from an external server was successful but failed to pass validation", internalMessage);
            }

            return response.Data;
        }

        #endregion
    }
}