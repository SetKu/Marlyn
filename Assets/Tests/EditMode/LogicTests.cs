using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Marlyn;

namespace Tests {
    public class BoardLogicTests {
        [Test]
        public void TestMakeMove() {
            Board board = new Board();
            // Example pawn move.
            Move move = new Move(board.PieceAt(new Vector2Int(0, 1)), new Vector2Int(0, 2));
            board.MakeMove(move);
            Assert.AreNotEqual(board.PieceAt(new Vector2Int(0, 2)), null);
            Assert.AreEqual(board.PieceAt(new Vector2Int(0, 1)), null);
        }

        [Test]
        public void TestUndoMove() {
            Board board = new Board();

            Assert.AreNotEqual(board.PieceAt(new Vector2Int(0, 1)), null, "The board doesn't seem to have been set correctly.");

            // Example pawn move.
            Move move = new Move(board.PieceAt(new Vector2Int(0, 1)), new Vector2Int(0, 2));

            List<Vector2Int> originalPositions = new List<Vector2Int>();
            foreach (Piece piece in board.pieces) {
                originalPositions.Add(piece.position);
            }
            
            board.MakeMove(move);
            Assert.AreNotEqual(board.PieceAt(new Vector2Int(0, 2)), null);
            Assert.AreEqual(board.PieceAt(new Vector2Int(0, 1)), null);
            board.UndoMove(move);

            List<Vector2Int> newPositions = new List<Vector2Int>();
            foreach (Piece piece in board.pieces) {
                Assert.AreNotEqual(piece, null, "A piece is null in 'board.pieces'.");
                newPositions.Add(piece.position);
            }

            Assert.AreEqual(originalPositions, newPositions);
        }

        [Test]
        public void TestCastlingQueenside() {
            Board board = new Board();

            Piece king = board.GetKing(Piece.Color.Black);
            Assert.AreEqual(board.GetLegalMoves(king).Count, 0, "Why does the black king have a move at the start of the game?");

            Move knightMove = new Move(board.PieceAt(new Vector2Int(1, 0)), new Vector2Int(0, 2));
            board.MakeMove(knightMove);

            Assert.AreEqual(board.GetLegalMoves(king).Count, 0, "The black king should not be able to make a move after only a knight has moved.");

            Move pawnMove = new Move(board.PieceAt(new Vector2Int(1, 1)), new Vector2Int(1, 2));
            Move bishopMove = new Move(board.PieceAt(new Vector2Int(2, 0)), new Vector2Int(0, 2));
            board.MakeMove(pawnMove);
            board.MakeMove(bishopMove);

            Assert.AreEqual(board.GetLegalMoves(king).Count, 0, "The black king should not be able to move when boxed in.");

            Piece queen = board.PieceAt(new Vector2Int(3, 0));
            Move queenMove1 = new Move(queen, new Vector2Int(1, 0));
            Move queenMove2 = new Move(queen, new Vector2Int(1, 1));
            board.MakeMove(queenMove1);
            board.MakeMove(queenMove2);

            Assert.AreEqual(board.GetLegalMoves(king).Count, 2, "There is a problem if castling queenside isn't possible for the black king.");
        }

        [Test]
        public void TestCastlingKingside() {
            Board board = new Board();

            Piece king = board.GetKing(Piece.Color.Black);
            Assert.AreEqual(board.GetLegalMoves(king).Count, 0, "Why does the black king have a move at the start of the game?");

            Move knightMove = new Move(board.PieceAt(new Vector2Int(6, 0)), new Vector2Int(5, 2));
            board.MakeMove(knightMove);

            Assert.AreEqual(board.GetLegalMoves(king).Count, 0, "The black king should not be able to make a move after only a knight has moved.");

            Move pawnMove = new Move(board.PieceAt(new Vector2Int(6, 1)), new Vector2Int(6, 2));
            Move bishopMove = new Move(board.PieceAt(new Vector2Int(5, 0)), new Vector2Int(7, 2));
            board.MakeMove(pawnMove);
            board.MakeMove(bishopMove);

            Assert.AreEqual(board.GetLegalMoves(king).Count, 2, "There is a problem if castling kingside isn't possible for the black king.");
        }

        [Test]
        public void TestPawnCapture() {
            Board board = new Board();

            // Left Side

            Piece pawn1 = board.PieceAt(new Vector2Int(0, 1));
            Move move1 = new Move(pawn1, new Vector2Int(0, 3));
            Move move2 = new Move(pawn1, new Vector2Int(0, 4));
            Move move3 = new Move(pawn1, new Vector2Int(0, 5));
            Move[] moves1 = new Move[] { move1, move2, move3 };

            foreach (Move move in moves1) {
                board.MakeMove(move);
            }

            List<Move> options1 = board.GetLegalMoves(pawn1);
            Assert.AreEqual(1, options1.Count);
            Assert.AreEqual(new Vector2Int(1, 6), options1[0].destination);

            // Right Side

            Piece pawn2 = board.PieceAt(new Vector2Int(7, 1));
            Move move4 = new Move(pawn2, new Vector2Int(7, 3));
            Move move5 = new Move(pawn2, new Vector2Int(7, 4));
            Move move6 = new Move(pawn2, new Vector2Int(7, 5));
            Move[] moves2 = new Move[] { move4, move5, move6 };

            foreach (Move move in moves2) {
                board.MakeMove(move);
            }

            List<Move> options2 = board.GetLegalMoves(pawn2);
            Assert.AreEqual(1, options2.Count);
            Assert.AreEqual(new Vector2Int(6, 6), options2[0].destination);
        }

        [Test]
        public void TestCheckStatus() {
            Board board = new Board();

            // Move pawn forward (by 2)
            Move move1 = new Move(board.PieceAt(new Vector2Int(4, 6)), new Vector2Int(4, 4));
            // Move king forward behind pawn
            Piece defKing = board.PieceAt(new Vector2Int(4, 7));
            Move move2 = new Move(defKing, new Vector2Int(4, 6));

            // Move pawn forward to get opposing color's queen out
            Move move3 = new Move(board.PieceAt(new Vector2Int(4, 1)), new Vector2Int(4, 2));
            // Move queen out
            Piece attQueen = board.PieceAt(new Vector2Int(3, 0));
            Move move4 = new Move(attQueen, new Vector2Int(7, 4));
            // Check opposing king from earlier with queen
            Move move5 = new Move(attQueen, new Vector2Int(7, 3));

            (Board.CheckInfo white, Board.CheckInfo black) checkStats1 = board.GetCheckStatus();
            Board.CheckInfo checkStat1 = defKing.color == Piece.Color.White ? checkStats1.white : checkStats1.black;
            Assert.AreEqual(false, checkStat1.isCheck, "King shouldn't be in check.");

            foreach (Move move in new Move[] { move1, move2, move3, move4, move5 }) {
                board.MakeMove(move);
            }

            (Board.CheckInfo white, Board.CheckInfo black) checkStats2 = board.GetCheckStatus();
            Board.CheckInfo checkStat2 = defKing.color == Piece.Color.White ? checkStats2.white : checkStats2.black;

            Assert.AreEqual(true, board.IsTileUnderAttack(defKing.position, attQueen.color), "The king should be under attack. There is a problem in `IsTileUnderAttack`.");
            Assert.AreEqual(true, checkStat2.isCheck, "King should be in check. If this is wrong, there is a problem in the check status but not `IsTileUnderAttack` logic.");
        }

        [Test]
        public void TestCheckEscapes() {
            Board board = new Board();

            // Move pawn forward (by 2)
            Move move1 = new Move(board.PieceAt(new Vector2Int(4, 6)), new Vector2Int(4, 4));
            // Move king forward behind pawn
            Piece defKing = board.PieceAt(new Vector2Int(4, 7));
            Move move2 = new Move(defKing, new Vector2Int(4, 6));

            // Move pawn forward to get opposing color's queen out
            Move move3 = new Move(board.PieceAt(new Vector2Int(4, 1)), new Vector2Int(4, 2));
            // Move queen out
            Piece attQueen = board.PieceAt(new Vector2Int(3, 0));
            Move move4 = new Move(attQueen, new Vector2Int(7, 4));
            // Check opposing king from earlier with queen
            Move move5 = new Move(attQueen, new Vector2Int(7, 3));

            foreach (Move move in new Move[] { move1, move2, move3, move4, move5 }) {
                board.MakeMove(move);
            }

            List<Move> kingEscapes = board.GetLegalMoves(defKing);

            foreach (Move escape in kingEscapes) {
                Assert.AreNotEqual(defKing.position, escape.destination, "The king cannot escape by staying where he is!");
            }

            Assert.AreEqual(3, kingEscapes.Count, "There should only be two escapes for the king himself.");

            Piece defKnight = board.PieceAt(new Vector2Int(6, 7));
            List<Move> defKnightMoves = board.GetLegalMoves(defKnight);
            Assert.AreEqual(1, defKnightMoves.Count, "The knight should only be able to move to block the queen's attack in the test.");
            Assert.AreEqual(new Vector2Int(5, 5), defKnightMoves[0].destination);
        }

        [Test]
        public void TestTileUnderAttackForPawns() {
            Board board = new Board();

            // Pawn Tests
            for (int x = 0; x < 8; x++) {
                Piece piece1 = board.PieceAt(new Vector2Int(x, 1));
                Assert.AreEqual(true, board.IsTileUnderAttack(new Vector2Int(x, 2), piece1.color));
                Piece piece2 = board.PieceAt(new Vector2Int(x, 6));
                Assert.AreEqual(true, board.IsTileUnderAttack(new Vector2Int(x, 5), piece2.color));
            }
        }

        [Test]
        public void TestKingLegalMoves() {
            Board board = new Board();
            Piece king = board.GetKing(Piece.Color.Black);
            king.position = new Vector2Int(4, 2);
            List<Move> moves1 = board.GetLegalMoves(king);
            Assert.AreEqual(5, moves1.Count, "The king should be able to move freely, except backwards, in this case.");
            king.position = new Vector2Int(4, 3);
            List<Move> moves2 = board.GetLegalMoves(king);
            Assert.AreEqual(8, moves2.Count, "The king should be able to move completely freely, in this case.");
        }

        [Test]
        public void TestGameNotOverAtStart() {
            Board board = new Board();
            Assert.AreEqual(false, board.GameOver());
        }
    }
}