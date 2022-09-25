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
    }
}