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
        private Vector3 dragScaleOffset = new Vector3(10, 10, 0);
        private bool validated = false;
        private int zOffset = -5;

        void OnMouseUp() {
            if (!validated) {
                return;
            }

            gameObject.transform.position -= new Vector3(0, 0, zOffset);
            gameObject.transform.localScale -= dragScaleOffset;

            if (piece != null) {
                if (closestTile != null && closestTile != game.BoardLocForPoint(originalPosition)) {
                    // IMPORTANT: Multiple moves to the same destination (like promotions) aren't handled.
                    // Lookup how to create a system dialog box.

                    foreach (Move move in legalMoves) {
                        if (move.destination == closestTile) {
                            game.MakeAndRenderMove(move);
                            ResetLegalTiles();
                            return;
                        }
                    }
                }

                // Reset piece to its original position automatically by rerendering state.
                gameObject.transform.position = originalPosition;

                ResetLegalTiles();
            }
        }

        void OnMouseDrag() {
            if (!validated) {
                return;
            }

            if (piece != null) {
                Vector3 newPos = GetMousePos() + dragOffset;
                newPos.z = transform.position.z;
                transform.position = newPos;
                closestTile = game.GetComponent<Game>().BoardLocForPoint(GetMousePos());
            }
        }

        void OnMouseDown() {
            if (game.board.nextMoveColor != piece.color) {
                validated = false;
                return;
            }

            validated = true;

            // Move the piece in front of all others.
            transform.position += new Vector3(0, 0, zOffset);
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
                GameObject tile = game.TileForLoc(move.destination);

                if (tile == null) {
                    return;
                }

                GameObject pieceObj = game.PieceForLoc(move.destination);

                bool alt = (move.destination.x + move.destination.y) % 2 == 0;

                if (pieceObj != null) {
                    Color current = pieceObj.GetComponent<SpriteRenderer>().color;
                    pieceObj.GetComponent<SpriteRenderer>().color = SF.Utils.DecreaseValue(current, 0.6f);
                }

                // White Case

                if (piece.color == Piece.Color.White) {
                    if (alt) {
                        tile.GetComponent<SpriteRenderer>().color = SF.Utils.DecreaseValue(whiteColor, 0.8f);
                    } else {
                        tile.GetComponent<SpriteRenderer>().color = whiteColor;
                    }

                    continue;
                }

                // Black Case
                
                if (alt) {
                    tile.GetComponent<SpriteRenderer>().color = SF.Utils.DecreaseValue(blackLegal, 0.8f);
                    continue;
                }

                tile.GetComponent<SpriteRenderer>().color = blackLegal;
            }
        }

        internal void ResetLegalTiles() {
            foreach (Move move in legalMoves) { 
                GameObject tile = game.TileForLoc(move.destination);

                if (tile == null) {
                    return;
                }

                GameObject pieceObj = game.PieceForLoc(move.destination);

                if (pieceObj != null) {
                    if (pieceObj.GetComponent<PieceHandler>().piece.color == Piece.Color.White) {
                        pieceObj.GetComponent<SpriteRenderer>().color = game.theme.whiteSet.piece;
                    } else {
                        pieceObj.GetComponent<SpriteRenderer>().color = game.theme.blackSet.piece;
                    }
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
