namespace PS.FreeBookHub_Lite.AuthService.API.Logging
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
        public const string UserRegistrationStarted =
            "Starting registration for email: {Email}";

        public const string UserRegistrationCompleted =
            "User successfully registered: {UserId}";

        public const string UserLoginAttempt =
            "Login attempt with email: {Email}";

        public const string UserLoginSuccess =
            "User logged in successfully: {UserId}";

        public const string UserLogout =
            "User {UserId} logged out";

        public const string UserLoggedOutAll =
            "User {UserId} logged out from all sessions";

        public const string TokenRefreshed =
            "Token refreshed for user: {UserId}";
    }
}
