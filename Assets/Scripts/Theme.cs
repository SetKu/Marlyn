using UnityEngine;

namespace Marlyn {
    [CreateAssetMenu(menuName = "ScriptableObjects/Theme")]
    public class Theme: ScriptableObject {
        public ColorSet whiteSet;
        public ColorSet blackSet;

        [System.Serializable]
        public struct ColorSet {
            public Color tile;
            public Color piece;
            // Legal move tile highlight
            public Color legal;
        }

        public GameObject rook, knight, bishop, queen, king, pawn, tile;

        internal GameObject Tile(Color color) {
            GameObject tile = Instantiate(this.tile);
            tile.GetComponent<SpriteRenderer>().color = color;
            return tile;
        }

        internal GameObject Piece(Piece.Type type, Piece.Color color) {
            GameObject piece = null;

            switch (type) {
                case Marlyn.Piece.Type.Rook:
                    piece = Instantiate(rook);
                    break;
                case Marlyn.Piece.Type.Knight:
                    piece = Instantiate(knight);
                    break;
                case Marlyn.Piece.Type.Bishop:
                    piece = Instantiate(bishop);
                    break;
                case Marlyn.Piece.Type.Queen:
                    piece = Instantiate(queen);
                    break;
                case Marlyn.Piece.Type.King:
                    piece = Instantiate(king);
                    break;
                case Marlyn.Piece.Type.Pawn:
                    piece = Instantiate(pawn);
                    break;
            }

            piece.GetComponent<SpriteRenderer>().color = (color == Marlyn.Piece.Color.White ? whiteSet.piece : blackSet.piece);
            return piece;
        }
    }
}