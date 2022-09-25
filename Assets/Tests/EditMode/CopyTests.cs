using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
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
            
            List<Vector2Int> originalPositions = new List<Vector2Int>();
            foreach (Piece piece in board.pieces) {
                originalPositions.Add(piece.position);
            }

            board.GetLegalMoves(board.pieces[0]);

            List<Vector2Int> newPositions = new List<Vector2Int>();
            foreach (Piece piece in board.pieces) {
                newPositions.Add(piece.position);
            }

            for (int i = 0; i < originalPositions.Count; i++) {
                Assert.AreEqual(originalPositions[i], newPositions[i]);
            }
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
