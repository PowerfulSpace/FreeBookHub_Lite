namespace PS.FreeBookHub_Lite.AuthService.Common.Logging
{
    public static class LoggerMessages
    {

        // ExceptionHandlingMiddleware
        //                  --- User Authentication Errors
        public const string InvalidUserIdentifier = "Invalid user ID format | Method: {Method} | Path: {Path}";
        public const string InvalidCredentials = "Invalid login attempt | Method: {Method} | Path: {Path}";
        public const string DeactivatedUser = "Attempt to access deactivated account — UserId: {UserId} | Method: {Method} | Path: {Path}";
        public const string UserAlreadyExists = "Attempt to register existing email — Email: {Email} | Method: {Method} | Path: {Path}";
        public const string UserNotFound = "User not found — UserId: {UserId} | Method: {Method} | Path: {Path}";

        //                  --- Token Management Errors
        public const string TokenNotFound = "Refresh token not found — Token: {Token} | Method: {Method} | Path: {Path}";
        public const string InvalidToken = "Invalid refresh token — Token: {Token} | Method: {Method} | Path: {Path}";
        public const string RevokedToken = "Attempt to use revoked token — Token: {Token} | Method: {Method} | Path: {Path}";

        //                  --- Role Management Errors
        public const string RoleManagementError = "Role management error | Method: {Method} | Path: {Path}";

        //                  --- General Error Handling
        public const string UnhandledException = "Unhandled exception — Message: {Message} | Method: {Method} | Path: {Path}";





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
