using System.Runtime.Serialization;

namespace WebTotalCommander.Core.Errors;

public class FileUnexpectedException : Exception
{
    public FileUnexpectedException() { }
    public FileUnexpectedException(string message) : base(message) { }
    public FileUnexpectedException(string message, Exception innerException) : base(message, innerException) { }
    public FileUnexpectedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
