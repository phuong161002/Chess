

public class User
{
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public string Avatar { get; set; }
    public int Level { get; set; }
    public long Amount { get; set; }

    public User(string username, string displayName, string avatar, int level, long amount)
    {
        Username = username;
        DisplayName = displayName;
        Avatar = avatar;
        Level = level;
        Amount = amount;
    }
}