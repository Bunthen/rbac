using System;

namespace cmedcc_idass.backend.Exceptions;

public class AppException : Exception
{
    public string ErrorCode { get; private set; } = string.Empty;
    public string OriginErrorCode { get; private set; } = string.Empty;

    public AppException()
    {
    }

    public AppException(string message) : base(message)
    {
    }

    public AppException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }

    public AppException(string errorCode, string originErrorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
        OriginErrorCode = originErrorCode;
    }

    public AppException(string errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

}