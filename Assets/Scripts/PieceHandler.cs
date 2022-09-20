using UnityEngine;
using System.Collections.Generic;

namespace Marlyn {
    public class PieceHandler: MonoBehaviour {
        internal Game game;
        internal Piece piece;
        private Camera mainCamera;
        private Vector3 dragOffset;
        private Vector3 originalPosition;
        private Vector2Int? closestTile = null;
        private List<Move> legalMoves = new List<Move>();
        private Vector3 dragScaleOffset = new Vector3(10, 10, 10);

        void OnMouseUp() {
            Vector3 pos = transform.position;
            pos.z = 1;
            transform.position = pos;

            gameObject.transform.localScale -= dragScaleOffset;

            if (piece != null) {
                if (closestTile != null && closestTile != game.ClosestTileToPoint(originalPosition)) {
                    game.MakeAndRenderMove(new Move(piece, closestTile.Value, null));
                    return;
                }

                game.RenderPieces();

                // foreach (Move move in legalMoves) {
                //     if (piece.color == Piece.Color.White) {
                //         game.TileForPoint(move.destination).GetComponent<SpriteRenderer>().color = game.theme.whiteSet.tile;
                //         continue;
                //     }
                    
                //     game.TileForPoint(move.destination).GetComponent<SpriteRenderer>().color = game.theme.blackSet.tile;
                // }

                // legalMoves = new List<Move>();
            }
        }

        void OnMouseDrag() {
            if (piece != null) {
                transform.position = GetMousePos() + dragOffset;
                closestTile = game.GetComponent<Game>().ClosestTileToPoint(GetMousePos());
            }
        }

        void OnMouseDown() {
            // Move the piece in front of all others (z = 1);
            Vector3 pos = transform.position;
            pos.z = 0;
            transform.position = pos;

            gameObject.transform.localScale += dragScaleOffset;

            if (mainCamera == null) {
                mainCamera = Camera.main;
            }

            if (piece != null) {
                originalPosition = transform.position;
                dragOffset = transform.position - GetMousePos();
                // legalMoves = game.board.GetLegalMoves(piece);
                // print(legalMoves);

                // foreach (Move move in legalMoves) {
                //     if (piece.color == Piece.Color.White) {
                //         game.TileForPoint(move.destination).GetComponent<SpriteRenderer>().color = game.theme.whiteSet.legal;
                //         continue;
                //     }
                    
                //     game.TileForPoint(move.destination).GetComponent<SpriteRenderer>().color = game.theme.blackSet.legal;
                // }
            }
        }

        Vector3 GetMousePos() {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            return mousePos;
        }
    }
}
