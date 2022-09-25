using UnityEngine;
using System;

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
        // These should not be changed.
        internal enum Color: int {
            White = -1,
            Black = 1
        }

        internal string id;
        internal Type type;
        internal Color color;
        internal Vector2Int position;
        internal bool hasMoved;

        internal Piece(Type type, Color color, Vector2Int position, bool hasMoved = false) {
            this.id = new Guid().ToString();
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