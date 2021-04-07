namespace Api.Configuration.Authentication
{
    internal class AuthenticationResult
    {
        public bool IsAuthenticated { get; set; }
        public int? UserSessionId { get; set; }
        public int? UserId { get; set; }
        public int? UserTypeId { get; set; }
    }
}