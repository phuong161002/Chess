using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

public class Search
{
    private const int immediateMateScore = 100000;
    private const int positiveInfinity = 9999999;
    private const int negativeInfinity = -positiveInfinity;
    private const int targetDepth = 4;

    private Move bestMove;
    private int bestEval;

    public event Action<Move> onSearchComplete;
    private Board board;
    private AIPlayer player;

    private int numPositions;
    public SearchDiagnostics searchDiagnostics;
    private Stopwatch searchStopwatch;
    private MoveOrdering moveOrdering;

    public Search(Board board, AIPlayer player, MoveGenerator moveGenerator)
    {
        this.board = board;
        this.player = player;
        moveOrdering = new MoveOrdering(moveGenerator);
    }

    private void InitDebugInfo()
    {
        searchStopwatch = Stopwatch.StartNew();
        numPositions = 0;
    }

    private void LogDebugInfo()
    {
        Debug.Log($"Best move: {bestMove.ToString()}  Eval: {bestEval} Num Eval: {searchDiagnostics.numPositionsEvaluated} NumPos: {numPositions} Search time: {searchStopwatch.ElapsedMilliseconds} ms");
    }

    public async void StartSearch()
    {
        InitDebugInfo();
        bestMove = Move.InvalidMove;
        await Task.Run(() => SearchMoves(targetDepth, 0, player, negativeInfinity, positiveInfinity));
        LogDebugInfo();
        onSearchComplete?.Invoke(bestMove);
    }
    
    private int SearchMoves(int depth, int plyFromRoot, ChessPlayer currentPlayer, int alpha, int beta)
    {
        if (depth == 0)
        {
            // return Evaluation.Evaluate(currentPlayer);
            return QuiescenceSearch(currentPlayer, 3, alpha, beta);
        }

        var moves = currentPlayer.GenerateMoves();
        
        if (moves.Count == 0)
        {
            if (currentPlayer.IsInCheck())
            {
                int mateScore = immediateMateScore - plyFromRoot;
                return -mateScore;
            }

            return 0;
        }

        moveOrdering.OrderMoves(moves);

        foreach (var move in moves)
        {
            // if(currentPlayer.IsMoveEnablingAttackOnPiece<King>(move))
            {
                board.MakeMove(move);
                numPositions++;
                int eval = - SearchMoves(depth - 1, plyFromRoot + 1, currentPlayer.opponent, -beta,
                    -alpha);
                board.UnmakeMove(move);
                if (eval >= beta)
                {
                    return beta;
                }

                if (eval > alpha)
                {
                    alpha = eval;
                    if (plyFromRoot == 0)
                    {
                        bestMove = move;
                        bestEval = alpha;
                    }
                }
            }
        }
        return alpha;
    }

    int QuiescenceSearch (ChessPlayer currentPlayer, int depth, int alpha, int beta) {
        // A player isn't forced to make a capture (typically), so see what the evaluation is without capturing anything.
        // This prevents situations where a player ony has bad captures available from being evaluated as bad,
        // when the player might have good non-capture moves available.
        
        int eval = Evaluation.Evaluate (currentPlayer);
        
        searchDiagnostics.numPositionsEvaluated++;
        if (eval >= beta) {
            return beta;
        }
        if (eval > alpha) {
            alpha = eval;
        }

        if (depth == 0)
        {
            return alpha;
        }

        var moves = currentPlayer.GenerateMoves (false);
        moveOrdering.OrderMoves (moves);
        
        for (int i = 0; i < moves.Count; i++) {
            board.MakeMove (moves[i], true);
            eval = -QuiescenceSearch (currentPlayer.opponent, depth - 1, -beta, -alpha);
            board.UnmakeMove (moves[i], true);

            if (eval >= beta) {
                return beta;
            }
            if (eval > alpha) {
                alpha = eval;
            }
        }

        return alpha;
    }

    public static bool IsMateScore (int score) {
        const int maxMateDepth = 1000;
        return System.Math.Abs (score) > immediateMateScore - maxMateDepth;
    }

    public static int NumPlyToMateFromScore (int score) {
        return immediateMateScore - System.Math.Abs (score);

    }   
    
    [Serializable]
    public class SearchDiagnostics {
        public int lastCompletedDepth;
        public string moveVal;
        public string move;
        public int eval;
        public bool isBook;
        public int numPositionsEvaluated;
    }
}