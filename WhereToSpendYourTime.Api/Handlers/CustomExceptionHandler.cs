using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Exceptions.Auth;
using WhereToSpendYourTime.Api.Exceptions.Categories;
using WhereToSpendYourTime.Api.Exceptions.Comments;
using WhereToSpendYourTime.Api.Exceptions.Items;
using WhereToSpendYourTime.Api.Exceptions.Media;
using WhereToSpendYourTime.Api.Exceptions.Reviews;
using WhereToSpendYourTime.Api.Exceptions.Tags;
using WhereToSpendYourTime.Api.Exceptions.Users;

namespace WhereToSpendYourTime.Api.Handlers;

/// <summary>
/// Global exception handler responsible for converting application
/// and domain exceptions into standardized HTTP responses
/// </summary>
/// <remarks>
/// Maps custom domain exceptions to appropriate HTTP status codes
/// and returns responses using the RFC7807 ProblemDetails format.
/// 
/// This handler centralizes API error handling and ensures
/// consistent error responses across all endpoints
/// </remarks>
public class CustomExceptionHandler : IExceptionHandler
{
    private readonly ILogger<CustomExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public CustomExceptionHandler(
        ILogger<CustomExceptionHandler> logger,
        IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    /// <summary>
    /// Attempts to handle an unhandled exception that occurred
    /// during request processing
    /// </summary>
    /// <remarks>
    /// Logs the exception, maps it to an HTTP status code,
    /// and returns a ProblemDetails response to the client.
    /// Returns true if the exception was handled
    /// </remarks>
    /// <param name="httpContext">
    /// The current HTTP context
    /// </param>
    /// <param name="exception">
    /// The thrown exception
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the request
    /// </param>
    /// <returns>
    /// True if the exception was handled and a response was written;
    /// otherwise false
    /// </returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred");

        var (statusCode, title) = exception switch
        {
            InvalidTagException => (
                StatusCodes.Status400BadRequest,
                "Invalid tag data"
            ),
            InvalidCommentException => (
                StatusCodes.Status400BadRequest,
                "Invalid comment data"
            ),
            InvalidCategoryException => (
                StatusCodes.Status400BadRequest,
                "Invalid category data"
            ),
            InvalidReviewException => (
                StatusCodes.Status400BadRequest,
                "Invalid review data"
            ),
            InvalidItemException => (
                StatusCodes.Status400BadRequest,
                "Invalid item data"
            ),
            InvalidRoleException => (
                StatusCodes.Status400BadRequest,
                "Invalid role"
            ),
            InvalidUserDisplayNameException => (
                StatusCodes.Status400BadRequest,
                "Invalid display name"
            ),
            PasswordChangeFailedException => (
                StatusCodes.Status400BadRequest,
                "Password change failed"
            ),
            UserProfileUpdateFailedException => (
                StatusCodes.Status400BadRequest,
                "User profile update failed"
            ),
            UserRoleUpdateFailedException => (
                StatusCodes.Status400BadRequest,
                "User role update failed"
            ),
            UserDeleteFailedException => (
                StatusCodes.Status400BadRequest,
                "User delete failed"
            ),
            InvalidRegisterRequestException => (
                StatusCodes.Status400BadRequest,
                "Invalid registration data"
            ),
            InvalidLoginRequestException => (
                StatusCodes.Status400BadRequest,
                "Invalid login data"
            ),
            InvalidPasswordResetRequestException => (
                StatusCodes.Status400BadRequest,
                "Invalid password reset data"
            ),
            InvalidPasswordResetTokenException => (
                StatusCodes.Status400BadRequest,
                "Invalid password reset token"
            ),
            RegisterFailedException => (
                StatusCodes.Status400BadRequest,
                "Registration failed"
            ),
            InvalidMediaException => (
                StatusCodes.Status400BadRequest,
                "Invalid media data"
            ),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized"
            ),
            InvalidCredentialsException => (
                StatusCodes.Status401Unauthorized,
                "Invalid credentials"
            ),

            CommentForbiddenException => (
                StatusCodes.Status403Forbidden,
                "Comment operation forbidden"
            ),
            ReviewForbiddenException => (
                StatusCodes.Status403Forbidden,
                "Review operation forbidden"
            ),
            DemoAccountOperationForbiddenException => (
                StatusCodes.Status403Forbidden,
                "DemoAccount operation forbidden"
            ),
            UserRoleForbiddenException => (
                StatusCodes.Status403Forbidden,
                "User role operation forbidden"
            ),
            UserDeleteForbiddenException => (
                StatusCodes.Status403Forbidden,
                "User operation forbidden"
            ),

            TagNotFoundException => (
                StatusCodes.Status404NotFound,
                "Tag not found"
            ),
            CommentNotFoundException => (
                StatusCodes.Status404NotFound,
                "Comment not found"
            ),
            CategoryNotFoundException => (
                StatusCodes.Status404NotFound,
                "Category not found"
            ),
            ReviewNotFoundException => (
                StatusCodes.Status404NotFound,
                "Review not found"
            ),
            UserItemReviewNotFoundException => (
                StatusCodes.Status404NotFound,
                "User item review not found"
            ),
            ItemNotFoundException => (
                StatusCodes.Status404NotFound,
                "Item not found"
            ),
            ItemTagNotFoundException => (
                StatusCodes.Status404NotFound,
                "Item tag not found"
            ),
            UserNotFoundException => (
                StatusCodes.Status404NotFound,
                "User not found"
            ),
            RoleNotFoundException => (
                StatusCodes.Status404NotFound,
                "Role not found"
            ),
            MediaNotFoundException => (
                StatusCodes.Status404NotFound,
                "Media not found"
            ),

            TagAlreadyExistsException => (
                StatusCodes.Status409Conflict,
                "Tag already exists"
            ),
            CategoryAlreadyExistsException => (
                StatusCodes.Status409Conflict,
                "Category already exists"
            ),
            ReviewAlreadyExistsException => (
                StatusCodes.Status409Conflict,
                "Review already exists"
            ),
            ItemTagAlreadyExistsException => (
                StatusCodes.Status409Conflict,
                "Item tag already exists"
            ),
            UserAlreadyExistsException => (
                StatusCodes.Status409Conflict,
                "User already exists"
            ),

            UserRoleAssignmentFailedException => (
                StatusCodes.Status500InternalServerError,
                "User role assignment failed"
            ),
            PasswordResetFailedException => (
                StatusCodes.Status500InternalServerError,
                "Password reset failed"
            ),
            MediaUploadFailedException => (
                StatusCodes.Status500InternalServerError,
                "Media upload failed"
            ),
            MediaDeleteFailedException => (
                StatusCodes.Status500InternalServerError,
                "Media delete failed"
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal server error"
            ),
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        string extErrors = "errors";
        if (exception is PasswordChangeFailedException pcfe)
        {
            problemDetails.Extensions[extErrors] = pcfe.Errors
                .Select(e => e.Description);
        }

        if (exception is UserRoleUpdateFailedException urufe)
        {
            problemDetails.Extensions[extErrors] = urufe.Errors
                .Select(e => e.Description);
        }

        if (exception is UserProfileUpdateFailedException upufe)
        {
            problemDetails.Extensions[extErrors] = upufe.Errors
                .Select(e => e.Description);
        }

        if (exception is UserDeleteFailedException udfe)
        {
            problemDetails.Extensions[extErrors] = udfe.Errors
                .Select(e => e.Description);
        }
        if (exception is RegisterFailedException rfe)
        {
            problemDetails.Extensions[extErrors] = rfe.Errors
                .Select(e => e.Description);
        }

        if (exception is UserRoleAssignmentFailedException urafe)
        {
            problemDetails.Extensions[extErrors] = urafe.Errors
                .Select(e => e.Description);
        }

        if (exception is PasswordResetFailedException prfe)
        {
            problemDetails.Extensions[extErrors] = prfe.Errors
                .Select(e => e.Description);
        }

        httpContext.Response.StatusCode = statusCode;

        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
                Exception = exception
            });
    }
}