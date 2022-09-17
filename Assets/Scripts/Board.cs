using UnityEngine;
using System.Collections.Generic;
using System;

namespace Marlyn {
    [Serializable]
    public class Board {
        internal List<Piece> pieces;

        internal Board() {
            Reset();
        }

        internal struct CheckInfo {
            internal bool isCheck;
            internal bool isCheckmate;
            internal bool isStalemate;

            internal CheckInfo(bool isCheck = false, bool isCheckmate = false, bool isStalemate = false) {
                this.isCheck = isCheck;
                this.isCheckmate = isCheckmate;
                this.isStalemate = isStalemate;
            }
        }

        internal (CheckInfo white, CheckInfo black) GetCheckStatus(bool lookForCheckmate = true) {
            (CheckInfo white, CheckInfo black) checkStatus = (new CheckInfo(), new CheckInfo());
            
            foreach (Piece.Color color in Enum.GetValues(typeof(Piece.Color))) {
                Piece king = GetKing(color);
                
                // This is an odd case. If the king is missing, then the game is over.
                // The context in which this func would be used is undefined.
                if (king == null) {
                    continue;
                }

                // The number of moves that the current color can make.
                // If the king is in check, these are the moves that put him out of check.
                // If the king is not in check, these are just all the moves that the
                // current color can make, still without putting the king in check.
                int escapesCount = 0;

                foreach (Piece piece in pieces) {
                    if (piece.color == color) {
                        escapesCount += GetLegalMoves(piece).Count;
                    }
                }

                if (IsTileUnderAttack(king.position, king.color)) {
                    switch (color) {
                    case Piece.Color.White:
                        // The white king, currenly being iterated on, is in check.
                        checkStatus.white.isCheck = true;
                        break;
                    case Piece.Color.Black:
                        // The black king, currenly being iterated on, is in check.
                        checkStatus.black.isCheck = true;
                        break;
                    }
                }

                if (escapesCount == 0) {
                    // Without any legal moves, the current color or its opponent
                    // can either be in checkmate or stalemate.
                    // If the king is in check, then the color is in checkmate.
                    // If the king is not in check, then the color is in stalemate.
                    switch (color) {
                    case Piece.Color.White:
                        checkStatus.white.isCheckmate = checkStatus.white.isCheck;
                        checkStatus.white.isStalemate = !checkStatus.white.isCheck;
                        break;
                    case Piece.Color.Black:
                        checkStatus.black.isCheckmate = checkStatus.black.isCheck;
                        checkStatus.black.isStalemate = !checkStatus.black.isCheck;
                        break;
                    }
                }
            }

            return checkStatus;
        }

        internal bool IsTileUnderAttack(Vector2Int position, Piece.Color color) {
            foreach (Piece piece in pieces) {
                // Skip over pieces that aren't the attacking party.
                if (piece.color != color) {
                    continue;
                }

                List<Move> moves = FilterBlocked(GetMovementPattern(piece));

                foreach (Move move in moves) {
                    if (move.destination == position) {
                        return true;
                    }
                }
            }

            return false;
        }
        
        // Filters out moves that put the piece's king in check/checkmate
        // or only returns moves the saves the piece's king from check.
        internal List<Move> GetLegalMoves(Piece piece) {
            List<Move> startingMoves = FilterBlocked(GetMovementPattern(piece));
            List<Move> legalMoves = new List<Move>();

            foreach (Move move in startingMoves) {
                Board hypotheticalBoard = this.Copy();
                hypotheticalBoard.MakeMove(move);
                Piece king = GetKing(piece.color);

                if (hypotheticalBoard.IsTileUnderAttack(king.position, king.color)) {
                    continue;
                }

                legalMoves.Add(move);
            }

            return startingMoves;
        }

        // Does not filter out attacks on kings.
        // The point of groups is to filter out
        // moves that are blocked by other pieces.
        //
        // This function filters out moves against pieces of one's own color.
        internal List<Move> FilterBlocked(List<List<Move>> groups) {
            List<Move> unblockedMoves = new List<Move>();

            // Groups represent directions
            foreach (List<Move> group in groups) {
                bool blocked = false;

                foreach (Move move in group) {
                    Piece victimPiece = PieceAt(move.destination);

                    if (victimPiece != null) {
                        if (victimPiece.color != move.piece.color) {
                            unblockedMoves.Add(move);
                        }

                        blocked = true;
                    }

                    if (!blocked) {
                        unblockedMoves.Add(move);
                        continue;
                    }

                    break;
                }
            }

            return unblockedMoves;
        }

        private List<Piece.Type> promotionTypes = new List<Piece.Type> {
            // Found from quick google search
            Piece.Type.Queen,
            Piece.Type.Rook,
            Piece.Type.Bishop,
            Piece.Type.Knight
        };

        // # Movement Patterns
        // Movement patterns do not account for invalid moves.
        // They simply contain the possible movements of a piece
        // relative to its position, given the rules of the game.
        //
        // Movement patterns do account for positions off the board.
        //
        // Movement patterns still need to be filtered for blocked moves and whether they cause checks/checkmates or prevent them.

        internal List<List<Move>> GetMovementPattern(Piece piece) {
            // Group moves by direction/their need to be filtered.
            List<List<Move>> groups = null;

            switch (piece.type) {
            case Piece.Type.Pawn:
                groups = GetPawnMovementPattern(piece);
                break;
            case Piece.Type.Knight:
                groups = GetKnightMovementPattern(piece);
                break;
            case Piece.Type.Bishop:
                groups = GetBishopMovementPattern(piece);
                break;
            case Piece.Type.Rook:
                groups = GetRookMovementPattern(piece);
                break;
            case Piece.Type.Queen:
                groups = GetQueenMovementPattern(piece);
                break;
            case Piece.Type.King:

                break;
            }

            return groups;
        }

        internal List<List<Move>> GetPawnMovementPattern(Piece piece) {
            List<List<Move>> groups = new List<List<Move>>();
            groups.Add(new List<Move>());

            // Forward motion
            if (piece.hasMoved) {
                // The pawn has moved, it can only move one space forward.
                Vector2Int positionForward = new Vector2Int(piece.position.x, piece.position.y + (int) piece.color);
                bool promotionPossible = (piece.position.y == 1 && piece.color == Piece.Color.White) || (piece.position.y == 6 && piece.color == Piece.Color.Black);

                if (promotionPossible) {
                    foreach (Piece.Type type in promotionTypes) {
                        groups[0].Add(new Move(piece, positionForward, type));
                    }
                } else {
                    groups[0].Add(new Move(piece, positionForward));
                }
            } else {
                // If the pawn is on the starting row, then it can move two spaces forward.
                groups[0].Add(new Move(piece, new Vector2Int(piece.position.x, piece.position.y + (int) piece.color)));
                groups[0].Add(new Move(piece, new Vector2Int(piece.position.x, piece.position.y + (int) piece.color * 2)));
            }
            
            groups.Add(new List<Move>());
            groups.Add(new List<Move>());

            // Diagonal capture
            List<Vector2Int> captureSpots = FilterToBoard(new List<Vector2Int> {
                new Vector2Int(piece.position.x + 1, piece.position.y + (int) piece.color),
                new Vector2Int(piece.position.x - 1, piece.position.y + (int) piece.color)
            });

            // Check if the pawn can capture a piece diagonally
            for (int i = 0; i < captureSpots.Count; i++) {
                Vector2Int spot = captureSpots[i];
                Piece victimPiece = PieceAt(spot);

                if (victimPiece != null) {
                    // Determine whether the caputure leads to a promotion
                    if (spot.y == 0 || spot.y == 7) {
                        foreach (Piece.Type type in promotionTypes) {
                            groups[i + 1].Add(new Move(piece, spot, type));
                        }

                        continue;
                    }

                    // If the code is here, the capture does not lead to a promotion
                    groups[0].Add(new Move(piece, spot));
                }
            }

            return groups;
        }

        internal List<List<Move>> GetKnightMovementPattern(Piece piece) {
            List<List<Move>> groups = new List<List<Move>>();

            // Knight moves in an L shape
            List<Vector2Int> possiblePositions = FilterToBoard(new List<Vector2Int>() {
                new Vector2Int(piece.position.x + 1, piece.position.y + 2),
                new Vector2Int(piece.position.x + 2, piece.position.y + 1),
                new Vector2Int(piece.position.x + 2, piece.position.y - 1),
                new Vector2Int(piece.position.x + 1, piece.position.y - 2),
                new Vector2Int(piece.position.x - 1, piece.position.y - 2),
                new Vector2Int(piece.position.x - 2, piece.position.y - 1),
                new Vector2Int(piece.position.x - 2, piece.position.y + 1),
                new Vector2Int(piece.position.x - 1, piece.position.y + 2)
            });

            possiblePositions.ForEach(position => {
                groups.Add(new List<Move>());
                groups[groups.Count - 1].Add(new Move(piece, position));
            });

            return groups;
        }

        internal List<List<Move>> GetBishopMovementPattern(Piece piece) {
            // Bishop moves in a diagonal line
            List<List<Move>> groups = new List<List<Move>>();
            List<List<Vector2Int>> diagonalPositions = new List<List<Vector2Int>>();

            DiagonalPositions(piece.position, 8).ForEach(diagonalSet => {
                List<Vector2Int> filtered = FilterToBoard(diagonalSet);

                if (filtered.Count > 0) {
                    groups.Add(new List<Move>());
                    
                    filtered.ForEach(position => {
                        groups[groups.Count - 1].Add(new Move(piece, position));
                    });
                }
            });

            return groups;
        }

        internal List<List<Move>> GetRookMovementPattern(Piece piece) {
            // Rook moves in a straight line
            List<List<Move>> groups = new List<List<Move>>();

            OrthogonalPositions(piece.position, 8).ForEach(straightSet => {
                List<Vector2Int> filtered = FilterToBoard(straightSet);

                if (filtered.Count > 0) {
                    groups.Add(new List<Move>());
                    
                    filtered.ForEach(position => {
                        groups[groups.Count - 1].Add(new Move(piece, position));
                    });
                }
            });

            return groups;
        }

        internal List<List<Move>> GetQueenMovementPattern(Piece piece) {
            List<List<Move>> groups = new List<List<Move>>();

            groups.AddRange(GetBishopMovementPattern(piece));
            groups.AddRange(GetRookMovementPattern(piece));

            return groups;
        }

        internal List<List<Move>> GetKingMovementPattern(Piece piece) {
            List<List<Move>> groups = new List<List<Move>>();

            Vector2Int[] possiblePositions = {
                new Vector2Int(piece.position.x + 1, piece.position.y + 1),
                new Vector2Int(piece.position.x + 1, piece.position.y),
                new Vector2Int(piece.position.x + 1, piece.position.y - 1),
                new Vector2Int(piece.position.x, piece.position.y + 1),
                new Vector2Int(piece.position.x, piece.position.y - 1),
                new Vector2Int(piece.position.x - 1, piece.position.y + 1),
                new Vector2Int(piece.position.x - 1, piece.position.y),
                new Vector2Int(piece.position.x - 1, piece.position.y - 1)
            };

            for (int i = 0; i < possiblePositions.Length; i++) {
                groups.Add(new List<Move>());
                groups[i].Add(new Move(piece, possiblePositions[i]));
            }

            // *Castling*
            //
            // In castling, the king moves two squares either to the left 
            // or right if the rook is on the same row and hasn't moved.
            // In turn, the rook moves to the square next to the king on the inside.
            //
            // - You cannot castle in check.
            // - The king also cannot pass through a square that is under attack.
            // - You can not castle if there are pieces between the king and the rook.
            // - You can not castle if the king or rook has already moved.

            if (!piece.hasMoved) {
                // Castling is now possible.

                // King must be starting on their home row.
                List<Piece> possibleRooks = new List<Piece>() {
                    PieceAt(new Vector2Int(0, piece.position.y)),
                    PieceAt(new Vector2Int(7, piece.position.y))
                };

                List<Piece> rooks = new List<Piece>();

                foreach (Piece possibleRook in possibleRooks) {
                    if (possibleRook.type == Piece.Type.Rook) {
                        if (possibleRook.color == piece.color) {
                            if (!possibleRook.hasMoved) {
                                // The king and rook can both castle.
                                // Now checks need to be done to see if there 
                                // are pieces in the way or if the tiles are under attack.
                                rooks.Add(possibleRook);
                            }
                        }
                    }
                }

                foreach (Piece rook in rooks) {
                    bool canCastle = true;


                }
            }

            return groups;
        }

        internal List<Vector2Int> FilterToBoard(List<Vector2Int> positions) {
            List<Vector2Int> filteredPositions = new List<Vector2Int>();

            foreach (Vector2Int position in positions) {
                if (position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8) {
                    filteredPositions.Add(position);
                }
            }

            return filteredPositions;
        }

        internal List<List<Vector2Int>> OrthogonalPositions(Vector2Int origin, int length) {
            List<List<Vector2Int>> groups = new List<List<Vector2Int>>();

            for (int i = 0; i < 4; i++) {
                groups.Add(new List<Vector2Int>());
            }

            for (int i = 1; i < length; i++) {
                groups[0].Add(new Vector2Int(origin.x + i, origin.y));
                groups[1].Add(new Vector2Int(origin.x - i, origin.y));
                groups[2].Add(new Vector2Int(origin.x, origin.y + i));
                groups[3].Add(new Vector2Int(origin.x, origin.y - i));
            }

            return groups;
        }

        internal List<List<Vector2Int>> DiagonalPositions(Vector2Int origin, int length) {
            List<List<Vector2Int>> groups = new List<List<Vector2Int>>();

            for (int i = 0; i < 4; i++) {
                groups.Add(new List<Vector2Int>());
            }

            for (int i = 1; i < length; i++) {
                groups[0].Add(new Vector2Int(origin.x + i, origin.y + i));
                groups[1].Add(new Vector2Int(origin.x - i, origin.y - i));
                groups[2].Add(new Vector2Int(origin.x + i, origin.y - i));
                groups[3].Add(new Vector2Int(origin.x - i, origin.y + i));
            }

            return groups;
        }

        internal Piece GetKing(Piece.Color color) {
            foreach (Piece piece in pieces) {
                if (piece.type == Piece.Type.King && piece.color == color) {
                    return piece;
                }
            }

            return null;
        }

        internal void Reset() {
            pieces = new List<Piece>();
            foreach (Piece.Color color in new Piece.Color[] { Piece.Color.White, Piece.Color.Black }) {
                // Add home row pieces.
                int home = color == Piece.Color.White ? 7 : 0;

                pieces.Add(new Piece(Piece.Type.Rook, color, new Vector2Int(0, home)));
                pieces.Add(new Piece(Piece.Type.Knight, color, new Vector2Int(1, home)));
                pieces.Add(new Piece(Piece.Type.Bishop, color, new Vector2Int(2, home)));
                pieces.Add(new Piece(Piece.Type.Queen, color, new Vector2Int(3, home)));
                pieces.Add(new Piece(Piece.Type.King, color, new Vector2Int(4, home)));
                pieces.Add(new Piece(Piece.Type.Bishop, color, new Vector2Int(5, home)));
                pieces.Add(new Piece(Piece.Type.Knight, color, new Vector2Int(6, home)));
                pieces.Add(new Piece(Piece.Type.Rook, color, new Vector2Int(7, home)));
 
                // Add the pawns
                for (int x = 0; x < 8; x++) {
                    // The pawn row could also be found by type casting the color to an int,
                    // which corresponds to the color's pawn's movement direction.
                    int row = color == Piece.Color.White ? 6 : 1;
                    pieces.Add(new Piece(Piece.Type.Pawn, color, new Vector2Int(x, row)));
                }
            }
        }

        internal Piece PieceAt(Vector2Int position) {
            foreach (Piece piece in pieces) {
                if (piece.position == position) {
                    return piece;
                }
            }

            return null;
        }

        internal void MakeMove(Move move) {
            Piece captured = PieceAt(move.destination);

            if (captured != null) {
                pieces.Remove(captured);
                move.caputuredPiece = captured;
            }

            move.piece.position = move.destination;

            if (move.promotion != null) {
                move.piece.type = move.promotion.Value;
            }
        }

        internal void UndoMove(Move move) {
            pieces.Add(move.caputuredPiece);
            move.piece.position = move.origin;

            if (move.promotion != null) {
                move.piece.type = Piece.Type.Pawn;
            }
        }

        internal Board Copy() {
            Board copy = new Board();
            copy.pieces = new List<Piece>(pieces);

            foreach (Piece piece in this.pieces) {
                copy.pieces.Add(piece.Copy());
            }

            return copy;
        }

        // /// <summary>
        // /// Load a chess notation string into the game in PGN format.
        // /// </summary>
        // /// <param name="notation">The chess notation string to load.</param>
        // /// https://www.chess.com/article/view/chess-notation
        // /// https://en.wikipedia.org/wiki/Portable_Game_Notation
        // internal void LoadNotation(string notation) {
        //     Regex regex = new Regex(@"\d.\.+ ");
        //     MatchCollection matches = regex.Matches(notation);

        //     foreach (Match match in matches) {
        //         Debug.Log(match.Value);
        //     }
        // }
    }
}