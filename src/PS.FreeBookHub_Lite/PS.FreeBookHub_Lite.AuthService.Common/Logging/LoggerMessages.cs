namespace PS.FreeBookHub_Lite.AuthService.Common.Logging
{
    public static class LoggerMessages
    {
        // Общие ошибки
        public const string UnhandledException =
            "Unhandled exception occurred while processing {Method} {Path}";

        public const string UnauthorizedAccess =
            "Unauthorized access attempt by user";

        public const string InvalidToken =
            "Invalid token received during authentication";

       
        // AuthBookService
        //                  --- RegisterAsync
        public const string RegistrationStarted = "Registration started — Email: {Email}";
        public const string RegistrationSuccess = "Registration successful — Email: {Email}, UserId: {UserId}";

        //                  --- LoginAsync
        public const string LoginStarted = "Login started — Email: {Email}";
        public const string LoginSuccess = "Login successful — Email: {Email}, UserId: {UserId}";

        //                  --- RefreshTokenAsync
        public const string RefreshStarted = "Refresh token process started — Token: {RefreshToken}";
        public const string RefreshOldTokenRevoked = "Old refresh token revoked — Token: {RefreshToken}";
        public const string RefreshNewTokenIssued = "New tokens issued — UserId: {UserId}";

        //                  --- LogoutCurrentSessionAsync
        public const string LogoutSessionStarted = "User attempting to logout current session — RefreshToken: {RefreshToken}";
        public const string LogoutTokenRevoked = "Logout successful: refresh token revoked — Token: {RefreshToken}";

        //                  --- LogoutAllSessionsAsync
        public const string LogoutAllSessionsStarted = "User {UserId} requested logout from all sessions";
        public const string LogoutAllSessionsCompleted = "All refresh tokens revoked for user {UserId}";
    }
}
