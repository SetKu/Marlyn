using UnityEngine;
using System.Collections.Generic;

namespace Marlyn {
    public class PromotionHandler: MonoBehaviour {
        public List<Move> moves;
        internal Game controller;

        public void ChooseQueen() {
            ChooseType(Piece.Type.Queen);
        }

        public void ChooseRook() {
            ChooseType(Piece.Type.Rook);
        }

        public void ChooseKnight() {
            ChooseType(Piece.Type.Knight);
        }

        public void ChooseBishop() {
            ChooseType(Piece.Type.Bishop);
        }

        public void ChooseType(Piece.Type type) {
            foreach (Move move in moves) {
                if (move.promotion == type) {
                    controller.EndPromotion();
                    controller.MakeAndRenderMove(move);
                    return;
                }
            }
        }

        public void CancelPromo() {
            controller.EndPromotion();
        }
    }
}