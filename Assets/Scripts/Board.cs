using UnityEngine;
using System.Collections.Generic;

namespace Marlyn {
    public class Board {
        internal List<Piece> pieces;

        internal Board() {
            Reset();
        }

        internal List<Move> GetLegalMoves(Piece piece) {
            List<Move> legalMoves = new List<Move>();

            foreach (Move move in GetUnrestrictedMoves(piece)) {
                Piece victimPiece = PieceAt(move.destination);

                if (victimPiece == null) {
                    legalMoves.Add(move);
                } else if (victimPiece.color != piece.color) {
                    if (victimPiece.type != Piece.Type.King) {
                        // if (IsInCheck(piece.color)) {

                        // }
                    }
                }
            }

            return legalMoves;
        }
        
        internal bool IsInCheck(Piece.Color color) {
            Piece king = GetKing(color);
            List<Move> moves = GetUnrestrictedMoves(king);

            foreach (Move move in moves) {
                Piece victimPiece = PieceAt(move.destination);

                if (victimPiece != null) {
                    if (victimPiece.color == color) {
                        if (victimPiece.type == Piece.Type.King) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        internal List<Move> GetUnrestrictedMoves(Piece piece) {
            List<Move> unrestrictedMoves = new List<Move>();

            // Find all available moves for a given piece based on its type and position.


            return unrestrictedMoves;
        }

        internal Piece GetKing(Piece.Color color) {
            foreach (Piece piece in pieces) {
                if (piece.type == Piece.Type.King && piece.color == color) {
                    return piece;
                }
            }

            return null;
        }

        internal void Reset() {
            pieces = new List<Piece>();
            foreach (Piece.Color color in new Piece.Color[] { Piece.Color.White, Piece.Color.Black }) {
                // Add home row pieces.
                int home = color == Piece.Color.White ? 7 : 0;

                pieces.Add(new Piece(Piece.Type.Rook, color, new Vector2Int(0, home)));
                pieces.Add(new Piece(Piece.Type.Knight, color, new Vector2Int(1, home)));
                pieces.Add(new Piece(Piece.Type.Bishop, color, new Vector2Int(2, home)));
                pieces.Add(new Piece(Piece.Type.Queen, color, new Vector2Int(3, home)));
                pieces.Add(new Piece(Piece.Type.King, color, new Vector2Int(4, home)));
                pieces.Add(new Piece(Piece.Type.Bishop, color, new Vector2Int(5, home)));
                pieces.Add(new Piece(Piece.Type.Knight, color, new Vector2Int(6, home)));
                pieces.Add(new Piece(Piece.Type.Rook, color, new Vector2Int(7, home)));
 
                // Add the pawns
                for (int x = 0; x < 8; x++) {
                    int row = color == Piece.Color.White ? 6 : 1;
                    pieces.Add(new Piece(Piece.Type.Pawn, color, new Vector2Int(x, row)));
                }
            }
        }

        internal Piece PieceAt(Vector2Int position) {
            foreach (Piece piece in pieces) {
                if (piece.position == position) {
                    return piece;
                }
            }

            return null;
        }

        internal void MakeMove(Move move) {
            move.piece.position = move.destination;

            if (move.promotion != null) {
                move.piece.type = move.promotion.Value;
            }
        }

        internal void UndoMove(Move move) {
            move.piece.position = move.origin;

            if (move.promotion != null) {
                move.piece.type = Piece.Type.Pawn;
            }
        }

        // /// <summary>
        // /// Load a chess notation string into the game in PGN format.
        // /// </summary>
        // /// <param name="notation">The chess notation string to load.</param>
        // /// https://www.chess.com/article/view/chess-notation
        // /// https://en.wikipedia.org/wiki/Portable_Game_Notation
        // internal void LoadNotation(string notation) {
        //     Regex regex = new Regex(@"\d.\.+ ");
        //     MatchCollection matches = regex.Matches(notation);

        //     foreach (Match match in matches) {
        //         Debug.Log(match.Value);
        //     }
        // }
    }
}