using UnityEngine;

namespace Marlyn {
    public class Move {
        public Piece piece;
        public Vector2Int origin;
        public Vector2Int destination;
        public Piece.Type? promotion;
        
        public Piece caputuredPiece = null;
        public bool switchBackHasMoved = false;

        // Having a set castling type guarantees that the corresponding rook exists to execute the move.
        public CastlingType? castlingType = null;

        public enum CastlingType {
            Kingside,
            Queenside
        }

        public Move(Piece piece, Vector2Int destination, Piece.Type? promotion = null) {
            this.piece = piece;
            this.origin = piece.position;
            this.destination = destination;
            this.promotion = promotion;
        }
    }
}