
using System.Collections.Generic;
using System.Linq;

public class Room
{
    public string Id { get; set; }
    public RoomType RoomType { get; set; }
    public List<UserInfo> Players { get; set; }

    public UserInfo GetMyUserInfo()
    {
        var user = GameManager.Instance.User;
        if (user == null)
        {
            return default;
        }
        return Players.FirstOrDefault(userInfo => userInfo.Username == user.Username);
    }

    public UserInfo GetOpponentUserInfo()
    {
        var user = GameManager.Instance.User;
        if (user == null)
        {
            return default;
        }
        return Players.FirstOrDefault(userInfo => userInfo.Username != user.Username);
    }

    public UserInfo GetHostInfo() => Players.Count > 0 ? Players[0] : default;
}