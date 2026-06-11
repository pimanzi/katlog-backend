namespace katlog_backend.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message) { }
}