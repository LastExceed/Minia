using System.Collections.Generic;
using System.Drawing;

namespace Minia.Judge {
    class JudgeMeter {
        static List<float> recent;

        public static void Load() {
            recent = new List<float>();
        }

        public static void Judge(double offset) {
            recent.Add((float)(offset / Config.hitWindow) * Config.judgeMeterScale);
            if (recent.Count > Config.judgeMeterMaxCount) recent.RemoveAt(0);
        }

        public static void Draw() {
            foreach (float stamp in recent) Shapes.Line(stamp, 1f, stamp, 0.93f, Color.White);
            Shapes.Line(0f, 1f, 0f, 0.9f, Color.Red);
            Shapes.Line(Config.judgeMeterScale, 1f, Config.judgeMeterScale, 0.9f, Color.Red);
            Shapes.Line(-Config.judgeMeterScale, 1f, -Config.judgeMeterScale, 0.9f, Color.Red);
            var avg = (float)(Score.Average / Config.hitWindow) * Config.judgeMeterScale;
            Shapes.Line(avg, 1f, avg, 0.9f, Color.Orange);
        }
    }
}
