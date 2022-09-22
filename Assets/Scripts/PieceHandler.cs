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
                    print("Looking for matching legal move to point not at original position.");

                    foreach (Move move in legalMoves) {
                        if (move.destination == closestTile) {
                            print("Made and rendered move.");
                            game.MakeAndRenderMove(move);
                            break;
                        }
                    }

                    closestTile = null;
                    ResetLegalTiles();
                    return;
                }

                // Reset piece to its original position automatically by rerendering state.
                game.RenderPieces();
                ResetLegalTiles();
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

                ShowLegalTiles();
            }
        }

        internal void ShowLegalTiles() {
            legalMoves = game.board.GetLegalMoves(piece);
            print($"{legalMoves.Count.ToString()} {legalMoves.ToString()}");

            foreach (Move move in legalMoves) {
                GameObject tile = game.TileForPoint(move.destination);

                if (tile == null) {
                    print($"S: Couldn't find a tile for this particular move... for some reason. {move.destination.ToString()}");
                    return;
                }

                if (piece.color == Piece.Color.White) {
                    tile.GetComponent<SpriteRenderer>().color = game.theme.whiteSet.legal;
                    continue;
                }
                
                tile.GetComponent<SpriteRenderer>().color = game.theme.blackSet.legal;
            }
        }

        internal void ResetLegalTiles() {
            foreach (Move move in legalMoves) { 
                GameObject tile = game.TileForPoint(move.destination);

                if (tile == null) {
                    print($"R: Couldn't find a tile for this particular move... for some reason. {move.destination.ToString()}");
                    return;
                }

                if ((move.destination.x + move.destination.y) % 2 == 0) {
                    tile.GetComponent<SpriteRenderer>().color = game.theme.whiteSet.tile;
                    continue;
                }
                
                tile.GetComponent<SpriteRenderer>().color = game.theme.blackSet.tile;
            }

            legalMoves = new List<Move>();
        }

        Vector3 GetMousePos() {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            return mousePos;
        }
    }
}
