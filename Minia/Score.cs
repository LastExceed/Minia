using System;

namespace Minia {
    static class Score {
        static double totalHitOffsetAbs = 0;
        static double totalHitOffset = 0;
        public static int Hits {get; private set; }
        public static int Misses {get; private set; }
        public static double Result {
            get => (totalHitOffsetAbs + Misses * Config.hitWindow) / (Hits + Misses);
        }
        public static double Average {
            get => totalHitOffset / Hits;
        }

        public static void Include(double offset) {
            var offsetAbs = Math.Abs(offset);
            if (offsetAbs < Config.hitWindow) {
                Hits++;
                totalHitOffsetAbs += offsetAbs;
                totalHitOffset += offset;
            }
            else Misses++;
        }
    }
}
