using UnityEngine;
using System;

namespace Marlyn {
    public class Piece {
        public enum Type {
            Pawn,
            Knight,
            Bishop,
            Rook,
            Queen,
            King
        }

        // Raw values corresponds to pawn movement direction.
        // These should not be changed.
        public enum Color: int {
            White = -1,
            Black = 1
        }

        public string id;
        public Type type;
        public Color color;
        public Vector2Int position;
        public bool hasMoved;

        public Piece(Type type, Color color, Vector2Int position, bool hasMoved = false) {
            // https://learn.microsoft.com/en-us/dotnet/api/system.guid.newguid?view=net-7.0
            this.id = Guid.NewGuid().ToString();
            this.type = type;
            this.color = color;
            this.position = position;
            this.hasMoved = hasMoved;
        }

        public Piece Copy() {
            return new Piece(type, color, position, hasMoved);
        }
    }
}