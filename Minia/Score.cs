using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minia {
    static class Score {
        static double hitOffsetStack = 0;
        static int notesPassed = 0;
        public static double Result {
            get => hitOffsetStack / notesPassed;
        }

        public static void Include(double offset) {
            hitOffsetStack += offset;
            notesPassed++;
        }
    }
}
