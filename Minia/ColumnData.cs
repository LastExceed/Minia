using System.Collections.Generic;

namespace Minia {
    class ColumnData {
        public int startPos = 0;
        public bool isHolding = false;
        public double holdEndTime = 0f;
        public List<Beatmap.HitObject> notes;
    }
}
