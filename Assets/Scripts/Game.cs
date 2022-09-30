using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace Marlyn {
    public class Game: MonoBehaviour {
        public TextMeshProUGUI capturedPiecesText;
        public TextMeshProUGUI nextTurnText;
        public TextMeshProUGUI checkStatusText;
        public AudioSource audioSource;
        public AudioClip moveSFX;
        public Theme theme;
        internal Board board;
        private List<(Vector2Int, GameObject)> tileObjects = new List<(Vector2Int, GameObject)>();
        private List<(Vector2Int, GameObject)> pieceObjects = new List<(Vector2Int, GameObject)>();
        private GameObject boardCanvas;
        private GameObject piecesCanvas;
        private Vector2 boardOffset = new Vector2(-3.5f, -3.5f);
        public bool flipColorSides;

        // Start is called before the first frame update
        public void Start() {
            boardCanvas = GameObject.Find("BoardCanvas");
            piecesCanvas = GameObject.Find("PiecesCanvas");
            board = new Board();
            SetupBoardUI();
            RenderPiecesUI();
            RenderTextUI();
        }

        internal void PlayMoveSFX() {
            audioSource.PlayOneShot(moveSFX, 0.5f);
        }

        internal void MakeAndRenderMove(Move move) {
            board.MakeMove(move);
            RenderPiecesUI();
            RenderTextUI();
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

        internal void RenderPiecesUI() {
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

                GameObject rTile = TileForLoc(MapByRenderLoc(piece.position));

                Vector3 scale = pieceObject.transform.localScale;
                float xS = (float) Math.Pow(sizeDelta.x / referenceRes.x, -1f);
                float yS = (float) Math.Pow(sizeDelta.y / referenceRes.y, -1f);
                scale *= xS > yS ? yS : xS;
                pieceObject.transform.localScale = scale;
                pieceObject.transform.localPosition = rTile.transform.localPosition;

                // Set z position.
                Vector3 posCopy = pieceObject.transform.localPosition;
                posCopy.z = 0;
                pieceObject.transform.localPosition = posCopy;

                pieceObjects.Add((piece.position, pieceObject));
            }
        }

        public void RenderTextUI() {
            capturedPiecesText.text = "Captured Pieces:\n\n";

            List<Piece> reversedCP = new List<Piece>();

            for (int i = board.capturedPieces.Count - 1; i > -1; i--) {
                reversedCP.Add(board.capturedPieces[i]);
            }

            foreach (Piece piece in reversedCP) {
                capturedPiecesText.text += $"{piece.color} {piece.type}\n";
            }

            nextTurnText.text = $"Next Turn:\n{board.nextMoveColor}";

            (Board.CheckInfo white, Board.CheckInfo black) checkInfo = board.GetCheckStatus();
            checkStatusText.text = "Check Status:\n";

            if (checkInfo.white.isStalemate) {
                checkStatusText.text += "Stalemate";
            } else {
                if (checkInfo.white.isCheck) {
                    if (checkInfo.white.isCheckmate) {
                        checkStatusText.text += "Checkmate for Black";
                    } else {
                        checkStatusText.text += "Check for Black";
                    }
                } else if (checkInfo.black.isCheck) {
                    if (checkInfo.black.isCheckmate) {
                        checkStatusText.text += "Checkmate for White";
                    } else {
                        checkStatusText.text += "Check for White";
                    }
                } else {
                    checkStatusText.text += "Nothing yet";
                }
            }
        }

        public void ToggleTextUIVisibility() {
            capturedPiecesText.gameObject.SetActive(!capturedPiecesText.gameObject.activeSelf);
            nextTurnText.gameObject.SetActive(!nextTurnText.gameObject.activeSelf);
            checkStatusText.gameObject.SetActive(!checkStatusText.gameObject.activeSelf);
        }

        public Vector2Int MapByRenderLoc(Vector2Int position) {
            if (flipColorSides) {
                return position;
            }

            return new Vector2Int(position.x, 7 - position.y);
        }

        public List<Vector2Int> MapAllByRenderLoc(List<Vector2Int> positions) {
            if (flipColorSides) {
                return positions;
            }

            List<Vector2Int> newPositions = new List<Vector2Int>();

            foreach (Vector2Int pos in positions) {
                newPositions.Add(new Vector2Int(pos.x, 7 - pos.y));
            }

            return newPositions;
        }
    }
}
