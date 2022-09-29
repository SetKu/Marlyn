using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace Marlyn {
    public class Game: MonoBehaviour {
        public Camera mainCamera;
        public AudioSource audioSource;
        public AudioClip moveSFX;
        public Theme theme;
        internal Board board;
        private List<(Vector2Int, GameObject)> tileObjects = new List<(Vector2Int, GameObject)>();
        private List<(Vector2Int, GameObject)> pieceObjects = new List<(Vector2Int, GameObject)>();
        private GameObject boardCanvas;
        private GameObject piecesCanvas;
        private Vector2 boardOffset = new Vector2(-3.5f, -3.5f);

        internal void PlayMoveSFX() {
            audioSource.PlayOneShot(moveSFX, 0.5f);
        }

        // Start is called before the first frame update
        public void Start() {
            boardCanvas = GameObject.Find("BoardCanvas");
            piecesCanvas = GameObject.Find("PiecesCanvas");
            board = new Board();
            SetupBoardUI();
            RenderPieces();
        }

        internal void MakeAndRenderMove(Move move) {
            board.MakeMove(move);
            RenderPieces();
            PlayMoveSFX();
        }

        internal GameObject TileForLoc(Vector2Int location) {
            foreach ((Vector2Int, GameObject) tupleSet in tileObjects) {
                if (tupleSet.Item1 == location) {
                    return tupleSet.Item2;
                }
            }

            return null;
        }

        internal GameObject PieceForLoc(Vector2Int location) {
            foreach ((Vector2Int, GameObject) tupleSet in pieceObjects) {
                if (tupleSet.Item1 == location) {
                    return tupleSet.Item2;
                }
            }

            return null;
        }

        internal Vector2Int? BoardLocForPoint(Vector3 point) {
            float closestDistance = float.MaxValue;
            Vector2Int? closestLoc = null;

            foreach ((Vector2Int, GameObject) tupleSet in tileObjects) {
                float distance = Vector2.Distance(point, tupleSet.Item2.transform.position);

                if (distance < closestDistance) {
                    closestDistance = distance;
                    closestLoc = tupleSet.Item1;
                }
            }

            // Don't return a tile if it's too far away.
            // This prevents the user from dragging a piece off the board and having it move.
            if (closestDistance > 0.9f) {
                return null;
            }

            return closestLoc;
        }

        internal void SetupBoardUI() {
            foreach (Transform child in boardCanvas.transform) {
                Destroy(child.gameObject);
            }

            tileObjects = new List<(Vector2Int, GameObject)>();

            for (int x = 0; x < 8; x++) {
                for (int y = 0; y < 8; y++) {
                    float xf = x + boardOffset.x;
                    float yf = y + boardOffset.y;
                    Vector3 position = new Vector3(xf, yf, 0);
                    Color color = (x + y) % 2 == 0 ? theme.whiteSet.tile : theme.blackSet.tile;
                    GameObject tile = theme.Tile(color);
                    // add tile as child of canvas
                    tile.transform.SetParent(boardCanvas.transform, true);
                    tile.transform.position = position;
                    tile.name = $"Tile@X{x}Y{y}";

                    Vector2Int logicalLoc = (new Vector2Int(x, y));
                    tileObjects.Add((logicalLoc, tile));

                    if (x == 0 || y == 0) {
                        Vector3 tileSize = tile.GetComponent<Renderer>().bounds.size;
                        List<TextMeshPro> textComponents = new List<TextMeshPro>();

                        if (x == 0) {
                            GameObject textObj = new GameObject($"RankLabel-{y + 1}");
                            TextMeshPro textComp = textObj.AddComponent<TextMeshPro>();
                            textComponents.Add(textComp);
                            textObj.transform.position = new Vector3(position.x - tileSize.x, position.y, 0);
                            textComp.text = $"{y + 1}";
                            textObj.transform.SetParent(boardCanvas.transform, true);
                            textObj.GetComponent<Renderer>().sortingLayerName = "Board";
                        }
                        
                        if (y == 0) {
                            GameObject textObj = new GameObject($"FileLabel-{x + 1}");
                            TextMeshPro textComp = textObj.AddComponent<TextMeshPro>();
                            textComponents.Add(textComp);
                            textObj.transform.position = new Vector3(position.x, position.y - tileSize.y, 0);
                            
                            switch (x) {
                                case 0:
                                    textComp.text = "a";
                                    break;
                                case 1:
                                    textComp.text = "b";
                                    break;
                                case 2:
                                    textComp.text = "c";
                                    break;
                                case 3: 
                                    textComp.text = "d";
                                    break;
                                case 4: 
                                    textComp.text = "e";
                                    break;
                                case 5:
                                    textComp.text = "f";
                                    break;
                                case 6: 
                                    textComp.text = "g";
                                    break;
                                case 7:
                                    textComp.text = "h";
                                    break;
                            }

                            textObj.transform.SetParent(boardCanvas.transform, true);
                            textObj.GetComponent<Renderer>().sortingLayerName = "Board";
                        }

                        foreach (TextMeshPro comp in textComponents) {
                            comp.color = Color.white;
                            comp.fontSize = 4;
                            comp.font = Resources.Load("Inter SDF", typeof(TMP_FontAsset)) as TMP_FontAsset;
                            comp.verticalAlignment = VerticalAlignmentOptions.Middle;
                            comp.horizontalAlignment = HorizontalAlignmentOptions.Center;
                        }
                    }
                }
            }
        }

        internal void RenderPieces() {
            for (int i = 0; i < piecesCanvas.transform.childCount; i++) {
                Destroy(piecesCanvas.transform.GetChild(i).gameObject);
            }

            pieceObjects = new List<(Vector2Int, GameObject)>();

            Vector2 referenceRes = piecesCanvas.GetComponent<CanvasScaler>().referenceResolution;
            Vector2 sizeDelta = piecesCanvas.GetComponent<RectTransform>().sizeDelta;

            // Update the pieces map.
            foreach (Piece piece in board.pieces) {
                GameObject pieceObject = this.theme.Piece(piece.type, piece.color);

                pieceObject.GetComponent<PieceHandler>().piece = piece;
                pieceObject.GetComponent<PieceHandler>().game = this;
                pieceObject.transform.SetParent(piecesCanvas.transform, true);
                pieceObject.name = $"Piece@X{piece.position.x}Y{piece.position.y}-{piece.type}-{piece.color}";

                GameObject rTile = TileForLoc(piece.position);

                Vector3 scale = pieceObject.transform.localScale;
                float xS = (float) Math.Pow(sizeDelta.x / referenceRes.x, -1f);
                float yS = (float) Math.Pow(sizeDelta.y / referenceRes.y, -1f);
                scale *= xS > yS ? yS : xS;
                pieceObject.transform.localScale = scale;
                pieceObject.transform.localPosition = rTile.transform.localPosition;

                pieceObjects.Add((piece.position, pieceObject));
            }
        }
    }
}
