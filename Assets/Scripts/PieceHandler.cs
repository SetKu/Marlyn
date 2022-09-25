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
                    // IMPORTANT: Multiple moves to the same destination (like promotions) aren't handled.
                    // Lookup how to create a system dialog box.

                    foreach (Move move in legalMoves) {
                        if (move.destination == closestTile) {
                            game.MakeAndRenderMove(move);
                            print($"Position after making and rendering move: {move.piece.position}, piece there: {game.board.PieceAt(move.piece.position).type}");
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

            Color whiteColor = game.theme.whiteSet.legal;
            Color blackLegal = game.theme.blackSet.legal;

            foreach (Move move in legalMoves) {
                GameObject tile = game.TileForPoint(move.destination);

                if (tile == null) {
                    return;
                }

                bool alt = (move.destination.x + move.destination.y) % 2 == 0;

                if (piece.color == Piece.Color.White) {
                    if (alt) {
                        tile.GetComponent<SpriteRenderer>().color = SF.Utils.DecreaseValue(whiteColor, 0.75f);
                    } else {
                        tile.GetComponent<SpriteRenderer>().color = whiteColor;
                    }

                    continue;
                }
                
                if (alt) {
                    tile.GetComponent<SpriteRenderer>().color = SF.Utils.DecreaseValue(blackLegal, 0.75f);
                    continue;
                }

                tile.GetComponent<SpriteRenderer>().color = blackLegal;
            }
        }

        internal void ResetLegalTiles() {
            foreach (Move move in legalMoves) { 
                GameObject tile = game.TileForPoint(move.destination);

                if (tile == null) {
                    return;
                }

                bool alt = (move.destination.x + move.destination.y) % 2 == 0;

                if (alt) {
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
