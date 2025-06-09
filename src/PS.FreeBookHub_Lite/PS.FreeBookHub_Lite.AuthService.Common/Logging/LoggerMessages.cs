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


        // AuthBookService
        //                  --- RegisterAsync
        public const string RegistrationStarted =
            "Registration started — Email: {Email}";

        public const string RegistrationUserAlreadyExists =
            "Registration failed — user with email {Email} already exists";

        public const string RegistrationSuccess =
            "Registration successful — Email: {Email}, UserId: {UserId}";

        //                  --- LoginAsync
        public const string LoginStarted =
            "Login started — Email: {Email}";
        
        public const string LoginUserNotFound =
            "Login failed — user not found for email: {Email}";

        public const string LoginUserInactive =
            "Login failed — user is deactivated: {Email}";

        public const string LoginInvalidPassword =
            "Login failed — invalid password for email: {Email}";

        public const string LoginSuccess =
            "Login successful — Email: {Email}, UserId: {UserId}";

        //                  --- RefreshTokenAsync
        public const string RefreshStarted =
            "Refresh token process started — Token: {RefreshToken}";

        public const string RefreshTokenInvalidOrExpired =
            "Refresh token is invalid or expired — Token: {RefreshToken}";

        public const string RefreshUserNotFoundOrInactive =
            "User not found or inactive during refresh — UserId: {UserId}";

        public const string RefreshOldTokenRevoked =
            "Old refresh token revoked — Token: {RefreshToken}";

        public const string RefreshNewTokenIssued =
            "New tokens issued — UserId: {UserId}";

        //                  --- LogoutCurrentSessionAsync
        public const string LogoutSessionStarted =
            "User attempting to logout current session — RefreshToken: {RefreshToken}";

        public const string LogoutTokenNotFound =
            "Logout failed: refresh token not found — Token: {RefreshToken}";

        public const string LogoutTokenAlreadyRevoked =
            "Logout skipped: refresh token already revoked or expired — Token: {RefreshToken}";

        public const string LogoutTokenRevoked =
            "Logout successful: refresh token revoked — Token: {RefreshToken}";

        //                  --- LogoutAllSessionsAsync
        public const string LogoutAllSessionsStarted =
            "User {UserId} requested logout from all sessions";

        public const string LogoutAllSessionsCompleted =
            "All refresh tokens revoked for user {UserId}";

    }
}
