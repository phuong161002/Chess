using System;

public static class EventHandler
{
    public static event Action<Piece> OnPieceMove;
    public static void CallOnPieceMove(Piece piece)
    {
        if (OnPieceMove != null)
        {
            OnPieceMove(piece);
        }
    }

    public static event Action<Piece> OnRemovePiece;
    public static void CallOnRemovePiece(Piece piece)
    {
        if (OnRemovePiece != null)
        {
            OnRemovePiece(piece);
        }
    }

    public static event Action OnBeginSwitchTurn;
    public static void CallOnBeginSwitchTurn()
    {
        if (OnBeginSwitchTurn != null)
        {
            OnBeginSwitchTurn();
        }
    }

    public static event Action OnEndSwitchTurn;
    public static void CallOnEndSwitchTurn()
    {
        if (OnEndSwitchTurn != null)
        {
            OnEndSwitchTurn();
        }
    }
}
