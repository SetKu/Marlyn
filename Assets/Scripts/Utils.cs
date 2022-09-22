using UnityEngine;
using System;

namespace SF {
    public class Utils {
        public static Color DecreaseValue(Color color, float factor) {
            Color q = new Color(color.r, color.g, color.b, color.a);
            q.r = Math.Min(255, q.r * factor);
            q.g = Math.Min(255, q.g * factor);
            q.b = Math.Min(255, q.b * factor);
            return q;
        }
    }
}