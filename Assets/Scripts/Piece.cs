using UnityEngine;

namespace Marlyn {
    public class Piece {
        internal enum Type {
            Pawn,
            Knight,
            Bishop,
            Rook,
            Queen,
            King
        }

        // Raw values corresponds to pawn movement direction.
        internal enum Color: int {
            White = -1,
            Black = 1
        }

        internal Type type;
        internal Color color;
        internal Vector2Int position;
        internal bool hasMoved;

        internal Piece(Type type, Color color, Vector2Int position, bool hasMoved = false) {
            this.type = type;
            this.color = color;
            this.position = position;
            this.hasMoved = hasMoved;
        }

        internal Piece Copy() {
            return new Piece(type, color, position, hasMoved);
        }
    }
}