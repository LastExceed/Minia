using System.Collections.Generic;
using System.Drawing;

namespace Minia {
    static class JudgeMeter {
        const byte history = 32;
        static List<float> recent = new List<float>();
        public static void Judge(double offset) {
            recent.Add((float)(offset / Game.hitwindow) * -0.5f);
            if (recent.Count > history) recent.RemoveAt(0);
        }
        public static void Draw() {
            for (int i = 1; i < recent.Count; i++) {
                byte c = (byte)(256 / history * i);
                Shapes.Line(recent[i], -1f, recent[i], -0.9f, c, c, c);
            }
            if (recent.Count != 0) Shapes.Line(recent[0], -1f, recent[0], -0.9f, Color.Cyan);
            Shapes.Line(0f, -1f, 0f, -0.9f, Color.Red);
        }
    }
}
