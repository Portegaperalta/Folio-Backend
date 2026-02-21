using Folio.Core.Domain.Entities;
using Folio.Core.Domain.Exceptions;
using Folio.Core.Domain.Exceptions.Bookmark;
using Folio.Core.Domain.Exceptions.Folder;
using Folio.Core.Domain.Exceptions.User;
using Folio.Infrastructure.Persistence;

namespace FolioWebAPI.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized Access"),
                UserNotFoundException => (StatusCodes.Status404NotFound, "User not found"),
                FolderNotFoundException => (StatusCodes.Status404NotFound, "Folder not found"),
                BookmarkNotFoundException => (StatusCodes.Status404NotFound, "Bookmark not found"),
                EmptyUsernameException => (StatusCodes.Status400BadRequest, "User name cannot be empty"),
                EmptyUserEmailException => (StatusCodes.Status400BadRequest, "User email cannot be empty"),
                EmptyUserPasswordHashException => (StatusCodes.Status400BadRequest, "User password hash cannot be empty"),
                EmptyFolderNameException => (StatusCodes.Status400BadRequest, "Folder name cannot be empty"),
                EmptyBookmarkNameException => (StatusCodes.Status400BadRequest, "Bookmark name cannot be empty"),
                EmptyBookmarkUrlException => (StatusCodes.Status400BadRequest, "Bookmark url cannot be empty"),
                ForbiddenAccessException => (StatusCodes.Status403Forbidden, "Forbidden Access"),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
            };

            if (statusCode >= 500)
            {
                _logger.LogError(exception, "Unhandled Exception at {Path}", context.Request.Path);
                await PersistServerErrorsAsync(context, exception);
            }
            else
            {
                _logger.LogWarning("Client error ({StatusCode}) at {Path}: {Message}", statusCode, context.Request.Path, message);
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsJsonAsync(new
            {
                error = message,
                status = statusCode
            });
        }

        private static async Task PersistServerErrorsAsync(HttpContext httpContext, Exception exception)
        {
            try
            {
                await using var scope = httpContext.RequestServices.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                dbContext.Errors.Add(new Error
                {
                    ErrorMessage = exception.Message,
                    StackTrace = exception.StackTrace,
                    Date = DateTime.UtcNow
                });

                await dbContext.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
