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
        public const string RegistrationStarted = "[AUTH] REGISTER started | Email:{Email}";
        public const string RegistrationSuccess = "[AUTH] REGISTER success | Email:{Email} | UserId:{UserId}";

        //                  --- LoginAsync
        public const string LoginStarted = "[AUTH] LOGIN started | Email:{Email}";
        public const string LoginSuccess = "[AUTH] LOGIN success | Email:{Email} | UserId:{UserId}";

        //                  --- RefreshTokenAsync
        public const string RefreshStarted = "[AUTH] REFRESH started | Token:{RefreshToken}";
        public const string RefreshOldTokenRevoked = "[AUTH] REFRESH old_token_revoked | Token:{RefreshToken}";
        public const string RefreshNewTokenIssued = "[AUTH] REFRESH new_tokens_issued | UserId:{UserId}";

        //                  --- LogoutCurrentSessionAsync
        public const string LogoutSessionStarted = "[AUTH] LOGOUT_SESSION started | Token:{RefreshToken}";
        public const string LogoutTokenRevoked = "[AUTH] LOGOUT_SESSION success | Token:{RefreshToken}";

        //                  --- LogoutAllSessionsAsync
        public const string LogoutAllSessionsStarted = "[AUTH] LOGOUT_ALL started | UserId:{UserId}";
        public const string LogoutAllSessionsCompleted = "[AUTH] LOGOUT_ALL success | UserId:{UserId}";
    }
}
