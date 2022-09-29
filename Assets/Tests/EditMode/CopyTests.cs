using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Marlyn;

namespace Tests {
    public class BoardCopyTests {
        // A Test behaves as an ordinary method
        [Test]
        public void PieceListIsSeparated() {
            // Use the Assert class to test conditions
            Board board = new Board();
            Board copy = board.Copy();
            int priorCount = copy.pieces.Count;
            board.pieces.RemoveAt(board.pieces.Count - 1);
            Assert.AreNotEqual(priorCount, board.pieces.Count);
        }

        [Test]
        public void PieceCopiesAreSeparated() {
            Board board = new Board();
            Board copy = board.Copy();

            string boardId = board.pieces[board.pieces.Count - 1].id;
            string copyId = copy.pieces[board.pieces.Count - 1].id;
            Assert.AreNotEqual(boardId, copyId);

            board.pieces[board.pieces.Count - 1].position += new Vector2Int(1, 0);
            int newX = board.pieces[board.pieces.Count - 1].position.x;
            int copyX = copy.pieces[copy.pieces.Count - 1].position.x;
            Assert.AreNotEqual(newX, copyX);
        }

        [Test]
        public void GetLegalMovesDoesntAffectBoard() {
            Board board = new Board();
            
            // Check for difference between original board and copy with get legal moves.

            int total1 = 0;
            foreach (Piece piece in board.pieces) {
                total1 += piece.position.x;
                total1 += piece.position.y;
            }

            foreach (Piece piece in board.pieces) {
                board.GetLegalMoves(piece);
            }

            int total2 = 0;
            foreach (Piece piece in board.pieces) {
                total2 += piece.position.x;
                total2 += piece.position.y;
            }

            Assert.AreEqual(total1, total2);
        }

        [Test]
        public void KingMovementPatDoesntAffectBoard() {
            Board board = new Board();
            
            // Check for difference between original board and copy with get legal moves.

            List<Vector2Int> originalPositions = new List<Vector2Int>();
            foreach (Piece piece in board.pieces) {
                originalPositions.Add(piece.position);
            }

            Piece whiteKing = board.GetKing(Piece.Color.White);
            board.GetKingMovementPattern(whiteKing);
            Piece blackKing = board.GetKing(Piece.Color.Black);
            board.GetKingMovementPattern(blackKing);

            List<Vector2Int> newPositions = new List<Vector2Int>();
            foreach (Piece piece in board.pieces) {
                newPositions.Add(piece.position);
            }

            for (int i = 0; i < originalPositions.Count; i++) {
                Assert.AreEqual(originalPositions[i], newPositions[i]);
            }
        }

        [Test]
        public void GetCheckStatusAffectsBoard() {
            Board board = new Board();

            int posTotal1 = 0;

            foreach (Piece piece in board.pieces) {
                posTotal1 += piece.position.x;
                posTotal1 += piece.position.y;
            }

            (Board.CheckInfo white, Board.CheckInfo black) checkStats = board.GetCheckStatus();

            int posTotal2 = 0;

            foreach (Piece piece in board.pieces) {
                posTotal2 += piece.position.x;
                posTotal2 += piece.position.y;
            }

            Assert.AreNotEqual(posTotal1, 0);
            Assert.AreEqual(posTotal1, posTotal2, "Getting the check status of the board is affecting piece positions!");
        }
    }

    public class PieceCopyTests {
        [Test]
        public void PieceCopyIsSuccessful() {
            Vector2Int origPos = new Vector2Int();
            Piece original = new Piece(Piece.Type.Pawn, Piece.Color.White, origPos);
            Piece copy = original.Copy();
            copy.position += new Vector2Int(1, 0);
            Assert.AreNotEqual(1, original.position.x);
        }
    }
}
