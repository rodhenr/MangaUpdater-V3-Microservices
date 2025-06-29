namespace MangaUpdater.Shared.Exceptions;

public class UserNotFoundException: Exception
{
    public UserNotFoundException(string username) : base($"User '{username}' not found.")
    {
    }
}