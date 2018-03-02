using System.Collections.Generic;
using System.Drawing;

namespace Minia {
    class ColumnData {
        public int startPos = 0;
        public double judgeTime = 0f;
        public Color judgeColor;
        public bool isHolding = false;
        public double holdEndTime = 0f;
        public List<Beatmap.HitObject> notes;
    }
}
