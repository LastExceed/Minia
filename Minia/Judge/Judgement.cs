using System;
using System.Collections.Generic;
using System.Drawing;

namespace Minia.Judge {
    static class Judgement {
        public static void Load(byte columns) {
            JudgeMeter.Load();
            JudgeHighlighting.Load(columns);
            Score.Load();
        }

        public static void Judge(double offset, double time, byte column) {
            JudgeHighlighting.Judge(offset, time, column);
            JudgeMeter.Judge(offset);
        }

        public static void Draw(byte columns, double time) {
            JudgeHighlighting.Draw(columns, time);
            JudgeMeter.Draw();
        }
    }
}
