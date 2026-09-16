using System.Text.Json;
using StudentRegistration.Application.Common.Exceptions;
using StudentRegistration.Application.DTOs;

namespace StudentRegistration.Api.Middleware;

public class ExceptionHandlingMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<ExceptionHandlingMiddleware> _logger;

  public ExceptionHandlingMiddleware(
      RequestDelegate next,
      ILogger<ExceptionHandlingMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (AppException ex)
    {
      _logger.LogWarning(ex, "Application exception: {Message}", ex.Message);
      await WriteResponseAsync(context, ex.StatusCode, ex.Message, ex.ErrorCode);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unhandled exception");
      await WriteResponseAsync(context, 500,
          "An unexpected error occurred. Please try again later.",
          "INTERNAL_ERROR");
    }
  }

  private static async Task WriteResponseAsync(
      HttpContext context, int statusCode, string message, string errorCode)
  {
    context.Response.StatusCode = statusCode;
    context.Response.ContentType = "application/json";

    var response = new ApiErrorResponse
    {
      StatusCode = statusCode,
      Message = message,
      ErrorCode = errorCode,
      TraceId = context.TraceIdentifier
    };

    var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });

    await context.Response.WriteAsync(json);
  }
}