using System;
using System.Collections.Generic;
using System.Drawing;

namespace Minia {
    static class Judgement {
        static List<float> recent = new List<float>();
        static double[] judgeTime;
        static Color[] judgeColor;
        public static void Load(byte columns) {
            judgeTime = new double[columns];
            judgeColor = new Color[columns];
        }

        public static void Judge(double offset, double time, byte column) {
            judgeTime[column] = time + Config.judgeVisibleTime;
            var offsetAbs = Math.Abs(offset);
            if (offsetAbs < 100f / 3f) judgeTime[column] = 0;
            else if (offsetAbs < 200f / 3f) judgeColor[column] = Color.Green;
            else if (offsetAbs < 100f) judgeColor[column] = Color.Yellow;
            else {
                judgeColor[column] = Color.Red;
                Audio.miss.Position = 0;
            }

            recent.Add((float)(offset / Config.hitWindow) * Config.judgeMeterScale);
            if (recent.Count > Config.judgeMeterMaxCount) recent.RemoveAt(0);

            Score.Include(offset);
        }
        public static void DrawJudgeHighlighting(byte column, double time) {
            if (judgeTime[column] > time) Shapes.Rectangle(
                Stage.GetColumnStart(column),
                1f,
                Stage.GetColumnStart(column) + Stage.ColumnWidth,
                -1f,
                judgeColor[column]
                );
        }
        public static void DrawJudgeMeter() {
            foreach (float stamp in recent) Shapes.Line(stamp, 1f, stamp, 0.93f, Color.White);
            Shapes.Line(0f, 1f, 0f, 0.9f, Color.Red);
            Shapes.Line(Config.judgeMeterScale, 1f, Config.judgeMeterScale, 0.9f, Color.Red);
            Shapes.Line(-Config.judgeMeterScale, 1f, -Config.judgeMeterScale, 0.9f, Color.Red);
            var avg = (float)(Score.Average / Config.hitWindow)*Config.judgeMeterScale;
            Shapes.Line(avg, 1f, avg, 0.9f, Color.Orange);
        }
    }
}
