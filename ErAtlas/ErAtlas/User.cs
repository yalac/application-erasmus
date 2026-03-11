namespace ErAtlas;

public class User
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // Stockage local en mémoire pour une demo simple.
    private static readonly List<User> RegisteredUsers =
    [
        new User { Username = "admin", Password = "password" }
    ];

    public static bool ValidateCredentials(string username, string password)
    {
        return RegisteredUsers.Any(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
            && u.Password == password);
    }

    public static bool Exists(string username)
    {
        return RegisteredUsers.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public static void Register(string username, string password)
    {
        RegisteredUsers.Add(new User { Username = username, Password = password });
    }
}
