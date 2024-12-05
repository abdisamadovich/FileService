using System.Runtime.Serialization;

namespace WebTotalCommander.Core.Errors;

public class FolderUnexpectedException : Exception
{
    public FolderUnexpectedException() { }
    public FolderUnexpectedException(string message) : base(message) { }
    public FolderUnexpectedException(string message, Exception innerException) : base(message, innerException) { }
    public FolderUnexpectedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
