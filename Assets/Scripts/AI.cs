using UnityEngine;
using System.Collections.Generic;
using System;

namespace Marlyn {
    public class AI {
        public Board board;
        public Mode mode;

        public enum Mode {
            Random,
            Tree
        }

        public Move Search(Piece.Color color, int? depth) {
            if (mode == Mode.Tree && depth != null) {
                Piece.Color oppColor = (color == Piece.Color.White ? Piece.Color.Black : Piece.Color.White);
            
                Board hypExtremeBoard = board.Copy();
                List<Move> movesMade = new List<Move>();

                for (int i = 0; i < depth; i++) {
                    // Examples:
                    // First depth (0) finds best move for color.
                    // Second depth (1) finds best move for opponent.
                    bool findOppMove = (i % 2 == 1);

                    List<(int, Move)> rankedMoves = new List<(int, Move)>();

                    foreach (Piece piece in board.pieces) {
                        if (findOppMove && piece.color == color) {
                            continue;
                        } else if (!findOppMove && piece.color != color) {
                            continue;
                        }

                        foreach (Move move in board.GetLegalMoves(piece)) {
                            Board copy = board.Copy();
                            copy.MakeMove(move);
                            // If finding best move for color, find move that hits the opponent the hardest.
                            // + Vice versa.
                            int eval = copy.Eval(findOppMove ? color : oppColor);
                            rankedMoves.Add((eval, move));
                        }
                    }

                    if (rankedMoves.Count == 0) {
                        // Checkmate/Stalemate.
                        if (findOppMove) {
                            // The opponent cannot make any moves.
                            // This is the best case scenario for the color (unless stalemate).
                            
                        }
                    }

                    rankedMoves.Sort();

                    if (!findOppMove) {
                        rankedMoves.Reverse();
                    }

                    hypExtremeBoard.MakeMove(rankedMoves[0].Item2);
                    movesMade.Add(rankedMoves[0].Item2);
                }

                // bool max = depth % 2 == 1;

                // List<(int, Move)> rankedMoves = new List<(int, Move)>();
            
                // foreach (Piece piece in board.pieces) {
                //     if (max) {
                //         if (piece.color == color) {
                //             continue;
                //         }
                //     }

                //     foreach (Move move in board.GetLegalMoves(piece)) {
                //         Board copy = board.Copy();
                //         copy.MakeMove(move);
                //         int eval = copy.Eval(color);
                //         rankedMoves.Add((eval, move));
                //     }
                // }

                // rankedMoves.Sort();

                // if (max) {
                //     rankedMoves.Reverse();
                // }

                // if (rankedMoves.Count > 0) {
                //     return rankedMoves[0].Item2;
                // }

                // return null;
            }

            // Only other mode is random.

            List<Move> moves = new List<Move>();

            foreach (Piece piece in board.pieces) {
                if (piece.color == color) {
                    moves.AddRange(board.GetLegalMoves(piece));
                }
            }

            System.Random rd = new System.Random();
            return moves[rd.Next(moves.Count)];
        }
    }
}