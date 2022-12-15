public enum TeamColor
{
    WHITE,
    BLACK
}

public enum PieceType
{
    Pawn,
    Knight,
    Rook, 
    Bishop,
    Queen,
    King,
    None
}

public enum GameState
{
    Init, 
    Play,
    Finish
}

public enum MoveFlag
{
    None,
    LeftCastling, 
    RightCastling,
    PawnEnPassant,
    PawnPromotion
}

public enum WsTags
{
    Invalid,
    Login,  
    Register,
    UserInfo,
    RoomInfo,
    CreateRoom,
    StartGame,
    FindGame,
    MovePiece,
    EndGame,
    RoomList,
    JoinRoom,
    ExitRoom,
    RestartGame,
}

public enum CanvasTags
{
    Login,
    Register,
    Lobby,
    PlayRoom,
    GameOver,
    None,
}

public enum DialogueType
{
    Info,
    Error,
    Warning
}

public enum PlayMode
{
    PvP,
    PvE
}

public enum SceneTags
{
    Lobby,
    Game
}

public enum RoomType
{
    BaseRoom, 
    Lobby,
    PlayRoom,
}