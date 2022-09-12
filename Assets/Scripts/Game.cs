using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace Marlyn {
    public class Game: MonoBehaviour {
        public Theme theme;
        internal Board board;
        private Vector2 boardOffset = new Vector2(-3.5f, -3.5f);
        private List<Vector3> tilePositions = new List<Vector3>();
        private List<GameObject> pieceObjects = new List<GameObject>();

        // Start is called before the first frame update
        public void Start() {
            board = new Board();
            SetupBoardUI();
            RenderPieces();
        }

        internal void MakeAndRenderMove(Move move) {
            board.MakeMove(move);
            RenderPieces();
        }

        internal Vector2Int ClosestTileToPoint(Vector3 point) {
            float closestDistance = float.MaxValue;
            Vector2Int closestTile = new Vector2Int(-1, -1);

            foreach (Vector3 tilePosition in tilePositions) {
                float distance = Vector3.Distance(point, tilePosition);

                if (distance < closestDistance) {
                    closestDistance = distance;
                    closestTile = new Vector2Int((int) (tilePosition.x - boardOffset.x), (int) (tilePosition.y - boardOffset.y));
                }
            }

            return closestTile;
        }

        internal void SetupBoardUI() {
            GameObject canvas = GameObject.Find("BoardCanvas");

            foreach (Transform child in canvas.transform) {
                Destroy(child.gameObject);
            }

            for (int x = 0; x < 8; x++) {
                for (int y = 0; y < 8; y++) {
                    float xf = x + boardOffset.x;
                    float yf = y + boardOffset.y;
                    Vector3 position = new Vector3(xf, yf, 0);
                    Color color = (x + y) % 2 == 0 ? theme.whiteSet.tile : theme.blackSet.tile;
                    GameObject tile = theme.Tile(color);
                    // add tile as child of canvas
                    tile.transform.SetParent(canvas.transform, true);
                    tile.transform.position = position;
                    tile.name = $"Tile@X{x}Y{y}";
                    this.tilePositions.Add(position);

                    if (x == 0 || y == 0) {
                        Vector3 tileSize = tile.GetComponent<Renderer>().bounds.size;
                        List<TextMeshPro> textComponents = new List<TextMeshPro>();

                        if (x == 0) {
                            GameObject textObj = new GameObject($"RankLabel-{y + 1}");
                            TextMeshPro textComp = textObj.AddComponent<TextMeshPro>();
                            textComponents.Add(textComp);
                            textObj.transform.position = new Vector3(position.x - tileSize.x, position.y, 0);
                            textComp.text = $"{y + 1}";
                            textObj.transform.SetParent(canvas.transform, true);
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

                            textObj.transform.SetParent(canvas.transform, true);
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
            GameObject canvas = GameObject.Find("PiecesCanvas");
            
            for (int i = 0; i < canvas.transform.childCount; i++) {
                Destroy(canvas.transform.GetChild(i).gameObject);
            }

            // Update the pieces map.
            foreach (Piece piece in board.pieces) {
                Vector3 position = new Vector3(piece.position.x + boardOffset.x, piece.position.y + boardOffset.y, 0);
                GameObject pieceObject = this.theme.Piece(piece.type, piece.color);
                pieceObject.GetComponent<PieceHandler>().piece = piece;
                pieceObject.GetComponent<PieceHandler>().game = this;
                pieceObject.transform.position = position;
                pieceObject.transform.SetParent(canvas.transform, true);
                pieceObject.name = $"Piece@X{piece.position.x}Y{piece.position.y}-{piece.type}-{piece.color}";
            }
        }
    }
}
