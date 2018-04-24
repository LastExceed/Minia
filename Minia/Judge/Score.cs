using System;

namespace Minia {
    static class Score {
        static double totalHitOffsetAbs;
        static double totalHitOffset;
        public static int Hits {get; private set; }
        public static int Misses {get; private set; }
        public static double Result {
            get => (totalHitOffsetAbs + Misses * Config.hitWindow) / (Hits + Misses);
        }
        public static double Average {
            get => Hits == 0 ? 0 : totalHitOffset / Hits;
        }

        public static void Load() {
            totalHitOffsetAbs = 0;
            totalHitOffset = 0;
            Hits = 0;
            Misses = 0;
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
