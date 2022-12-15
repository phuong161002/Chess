using System.Collections.Generic;

public struct RoomInfo
{
    public string Id { get; set; }
    public RoomType RoomType { get; set; }
    public List<UserInfo> Players { get; set; } 
}