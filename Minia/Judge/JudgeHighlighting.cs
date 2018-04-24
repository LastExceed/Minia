using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minia.Judge {
    class JudgeHighlighting {
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
                Audio.miss.CurrentTime = new TimeSpan(0);
            }

            Score.Include(offset);
        }
        public static void Draw(byte columns, double time) {
            for (int column = 0; column < columns; column++) {
                if (judgeTime[column] > time) {
                    Shapes.Rectangle(
                        Stage.GetColumnStart(column),
                        1f,
                        Stage.GetColumnStart(column) + Stage.ColumnWidth,
                        -1f,
                        judgeColor[column]
                    );
                }
            }
        }
    }
}
