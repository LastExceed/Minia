using System;
using System.Collections.Generic;
using System.Drawing;

namespace Minia {
    static class Judgement {
        static List<float> recent = new List<float>();
        static double[] judgeTime = new double[Config.columns];
        static Color[] judgeColor = new Color[Config.columns];

        public static void Judge(double offset, double time, byte column) {
            judgeTime[column] = time + Config.judgeVisibleTime;
            var offsetAbs = Math.Abs(offset);
            if (offsetAbs < 100f/3f) judgeTime[column] = 0;
            else if (offsetAbs < 200f/3f) judgeColor[column] = Color.Green;
            else if (offsetAbs < 100f) judgeColor[column] = Color.Yellow;
            else judgeColor[column] = Color.Red;

            recent.Add((float)(offset / Config.hitWindow) * Config.judgeMeterScale * -1f);
            if (recent.Count > Config.judgeMeterMaxCount) recent.RemoveAt(0);

            Score.Include(offsetAbs);
        }
        public static void DrawJudgeHighlighting(byte column, double time) {
            if (judgeTime[column] > time) Shapes.Rectangle(column / 2f - 1f, 1f, column / 2f - 0.5f, -1f, judgeColor[column]);
        }
        public static void DrawJudgeMeter() {
            for (int i = 0; i < recent.Count; i++) {
                byte c = (byte)(256 / Config.judgeMeterMaxCount * i);
                Shapes.Line(recent[i], -1f, recent[i], -0.93f, c, c, c);
            }
            Shapes.Line(0f, -1f, 0f, -0.9f, Color.Red);
        }
    }
}
