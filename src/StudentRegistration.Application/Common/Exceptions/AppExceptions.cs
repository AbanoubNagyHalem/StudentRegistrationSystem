namespace StudentRegistration.Application.Common.Exceptions;


public abstract class AppException : Exception
{
  public abstract int StatusCode { get; }
  public string ErrorCode { get; }

  protected AppException(string message, string errorCode) : base(message)
  {
    ErrorCode = errorCode;
  }
}

public class NotFoundException : AppException
{
  public override int StatusCode => 404;

  public NotFoundException(string message, string errorCode = "NOT_FOUND")
      : base(message, errorCode) { }
}

public class BusinessRuleException : AppException
{
  public override int StatusCode => 400;

  public BusinessRuleException(string message, string errorCode = "BUSINESS_RULE_VIOLATION")
      : base(message, errorCode) { }
}

public class ConflictException : AppException
{
  public override int StatusCode => 409;

  public ConflictException(string message, string errorCode = "CONFLICT")
      : base(message, errorCode) { }
}

public class UnauthorizedException : AppException
{
  public override int StatusCode => 401;

  public UnauthorizedException(string message, string errorCode = "UNAUTHORIZED")
      : base(message, errorCode) { }
}

public class ForbiddenException : AppException
{
  public override int StatusCode => 403;

  public ForbiddenException(string message, string errorCode = "FORBIDDEN")
      : base(message, errorCode) { }
}