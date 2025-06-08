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

        // AuthController
        //                  --- Register
        public const string UserRegistrationStarted =
            "Starting registration for email: {Email}";

        public const string UserRegistrationCompleted =
            "User successfully registered: {Email}";

        //                  --- Login
        public const string UserLoginAttempt =
            "Login attempt with email: {Email}";

        public const string UserLoginSuccess =
            "User logged in successfully: {Email}";

        //                  --- Logout
        public const string UserLogoutAttempt =
            "User {UserId} attempting to log out";

        public const string UserLogoutSuccess =
            "User {UserId} logged out successfully";

        //                  --- LogoutAll
        public const string UserLogoutAllAttempt =
            "User {UserId} attempting to log out from all sessions";

        public const string UserLogoutAllSuccess =
            "User {UserId} logged out from all sessions successfully";

        //                  --- Refresh
        public const string TokenRefreshAttempt =
            "Attempting token refresh for user: {UserId}";

        public const string TokenRefreshSuccess =
            "Token successfully refreshed for user: {UserId}";

        //                  --- GetUserIdFromClaimsOrThrow
        public const string UserIdExtractionAttempt =
            "Attempting to extract user ID from claims";

        public const string UserIdExtractionSuccess =
            "Successfully extracted user ID from claims: {UserId}";

        public const string UserIdExtractionFailed =
            "Failed to extract valid user ID from claims: {ClaimValue}";
    }
}
