using UnityEngine;

namespace Marlyn {
    public class Move {
        internal Piece piece;
        internal Vector2Int origin;
        internal Vector2Int destination;
        internal Piece.Type? promotion;

        internal Move(Piece piece, Vector2Int destination, Piece.Type? promotion) {
            this.piece = piece;
            this.origin = piece.position;
            this.destination = destination;
            this.promotion = promotion;
        }
    }
}