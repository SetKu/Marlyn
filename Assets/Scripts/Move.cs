using UnityEngine;

namespace Marlyn {
    public class Move {
        internal Piece piece;
        internal Vector2Int origin;
        internal Vector2Int destination;
        internal Piece.Type? promotion;
        internal Piece caputuredPiece = null;

        // Having a set castling type guarantees that the corresponding rook exists to execute the move.
        internal CastlingType? castlingType = null;

        public enum CastlingType {
            Kingside,
            Queenside
        }

        internal Move(Piece piece, Vector2Int destination, Piece.Type? promotion = null) {
            this.piece = piece;
            this.origin = piece.position;
            this.destination = destination;
            this.promotion = promotion;
        }
    }
}