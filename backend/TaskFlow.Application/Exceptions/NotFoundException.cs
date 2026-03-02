namespace TaskFlow.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} with id '{key}' was not found.")
    {
    }
}
