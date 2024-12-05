using System.Runtime.Serialization;

namespace WebTotalCommander.Core.Errors;

public class ParameterInvalidException : Exception
{
    public ParameterInvalidException() { }
    public ParameterInvalidException(string message) : base(message) { }
    public ParameterInvalidException(string message, Exception innerException) : base(message, innerException) { }
    protected ParameterInvalidException(SerializationInfo info, StreamingContext context)
       : base(info, context)
    {
    }
}
