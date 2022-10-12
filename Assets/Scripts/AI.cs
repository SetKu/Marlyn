using System.Collections.Generic;

namespace Marlyn {
    public class AI {
        public Board board;

        public Move RandomMove(Piece.Color color) {
            List<Move> moves = new List<Move>();

            foreach (Piece piece in board.pieces) {
                if (piece.color == color) {
                    moves.AddRange(board.GetLegalMoves(piece));
                }
            }

            if (moves.Count == 0) {
                return null;
            }

            System.Random rd = new System.Random();
            return moves[rd.Next(moves.Count)];
        }
    }
}