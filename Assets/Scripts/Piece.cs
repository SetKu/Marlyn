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

        internal enum Color {
            White,
            Black
        }

        internal Type type;
        internal Color color;
        internal Vector2Int position;
        internal bool hasMoved;

        internal Piece(Type type, Color color, Vector2Int position) {
            this.type = type;
            this.color = color;
            this.position = position;
            this.hasMoved = false;
        }
    }
}