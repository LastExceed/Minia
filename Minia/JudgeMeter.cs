using System.Collections.Generic;
using System.Drawing;

namespace Minia {
    static class JudgeMeter {
        static List<float> recent = new List<float>();
        public static void Judge(double offset) {
            recent.Add((float)(offset / Game.hitwindow) * -1f);
            if (recent.Count > 10) recent.RemoveAt(0);
        }
        public static void Draw() {
            for (int i = 1; i < recent.Count; i++) {
                Minia.Draw.Line(recent[i], -1f, recent[i], -0.9f, Color.Blue);
            }
            if (recent.Count != 0) Minia.Draw.Line(recent[0], -1f, recent[0], -0.9f, Color.Cyan);
            Minia.Draw.Line(0f, -1f, 0f, -0.9f, Color.Red);
        }
    }
}
