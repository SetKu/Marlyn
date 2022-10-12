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

        public Move TreeMove(Piece.Color color, int depth) {
            if (depth <= 0) {
                return null;
            }

            Piece.Color oppColor = (color == Piece.Color.White ? Piece.Color.Black : Piece.Color.White);
            Board baseBoard = board.Copy();
            int movesMade = 0;

            for (int i = 0; i < depth; i++) {
                // Minimize opponent eval (through home action) or not.
                bool min = i % 2 == 0;
                // Iteration color.
                Piece.Color iColor = (min ? oppColor : color);

                (Move move, int eval)? bestMove = null;
                bool shouldBreak = false;

                foreach (Piece piece in baseBoard.pieces) {
                    if (piece.color != iColor) { continue; }

                    foreach (Move move in board.GetLegalMoves(piece)) {
                        Board hypBoard = baseBoard.Copy();
                        hypBoard.MakeMove(move);

                        // Eval is always of opposing color.
                        // If on the first level of tree, the goal is to find the move the home color can play that minimizes the opponents eval.
                        int eval = hypBoard.Eval(min ? oppColor : color);

                        if (oppColor == Piece.Color.White ? hypBoard.GetCheckStatus().white.isCheckmate : hypBoard.GetCheckStatus().black.isCheckmate) {
                            // This is the best outcome. No further searching is required.
                            bestMove = (move, eval);
                            shouldBreak = true;
                            break;
                        }

                        if (bestMove != null) {
                            if (bestMove.Value.eval > eval) {
                                bestMove = (move, eval);
                            }

                            continue;
                        }

                        bestMove = (move, eval);
                    }

                    if (shouldBreak) {
                        break;
                    }
                }

                if (bestMove == null) {
                    break;
                }

                baseBoard.MakeMove(bestMove.Value.move);
                movesMade += 1;

                if (shouldBreak) {
                    break;
                }
            }

            if (movesMade > 0) {
                // First move.
                return baseBoard.movesMade[baseBoard.movesMade.Count - movesMade];
            }

            return null;
        }

        // public Move TreeMove(Piece.Color color, int depth) {
        //     if (mode == Mode.Tree && depth != null) {
        //         Piece.Color oppColor = (color == Piece.Color.White ? Piece.Color.Black : Piece.Color.White);
            
        //         Board hypExtremeBoard = board.Copy();
        //         List<Move> movesMade = new List<Move>();

        //         for (int i = 0; i < depth; i++) {
        //             // Examples:
        //             // First depth (0) finds best move for color.
        //             // Second depth (1) finds best move for opponent.
        //             bool findOppMove = (i % 2 == 1);

        //             List<(int, Move)> rankedMoves = new List<(int, Move)>();

        //             foreach (Piece piece in board.pieces) {
        //                 if (findOppMove && piece.color == color) {
        //                     continue;
        //                 } else if (!findOppMove && piece.color != color) {
        //                     continue;
        //                 }

        //                 foreach (Move move in board.GetLegalMoves(piece)) {
        //                     Board copy = board.Copy();
        //                     copy.MakeMove(move);
        //                     // If finding best move for color, find move that hits the opponent the hardest.
        //                     // + Vice versa.
        //                     int eval = copy.Eval(findOppMove ? color : oppColor);
        //                     rankedMoves.Add((eval, move));
        //                 }
        //             }

        //             if (rankedMoves.Count == 0) {
        //                 // Checkmate/Stalemate.
        //                 if (findOppMove) {
        //                     // The opponent cannot make any moves.
        //                     // This is the best case scenario for the color (unless stalemate).
                            
        //                 }
        //             }

        //             rankedMoves.Sort();

        //             if (!findOppMove) {
        //                 rankedMoves.Reverse();
        //             }

        //             hypExtremeBoard.MakeMove(rankedMoves[0].Item2);
        //             movesMade.Add(rankedMoves[0].Item2);
        //         }

        //         // bool max = depth % 2 == 1;

        //         // List<(int, Move)> rankedMoves = new List<(int, Move)>();
            
        //         // foreach (Piece piece in board.pieces) {
        //         //     if (max) {
        //         //         if (piece.color == color) {
        //         //             continue;
        //         //         }
        //         //     }

        //         //     foreach (Move move in board.GetLegalMoves(piece)) {
        //         //         Board copy = board.Copy();
        //         //         copy.MakeMove(move);
        //         //         int eval = copy.Eval(color);
        //         //         rankedMoves.Add((eval, move));
        //         //     }
        //         // }

        //         // rankedMoves.Sort();

        //         // if (max) {
        //         //     rankedMoves.Reverse();
        //         // }

        //         // if (rankedMoves.Count > 0) {
        //         //     return rankedMoves[0].Item2;
        //         // }

        //         // return null;
        //     }
        // }
    }
}