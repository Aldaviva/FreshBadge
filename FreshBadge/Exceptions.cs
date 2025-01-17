namespace FreshBadge;

public class FreshBadgeException(string message, Exception? cause = null): ApplicationException(message, cause);