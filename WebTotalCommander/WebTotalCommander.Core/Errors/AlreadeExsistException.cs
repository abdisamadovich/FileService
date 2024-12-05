using System.Runtime.Serialization;

namespace WebTotalCommander.Core.Errors;

public class AlreadeExsistException : Exception
{
    public AlreadeExsistException() { }
    public AlreadeExsistException(string message) : base(message) { }
    public AlreadeExsistException(string message, Exception innerException) : base(message, innerException) { }
    public AlreadeExsistException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
