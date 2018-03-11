using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Minia {
    class Beatmap {
        public class TimingPoint {
            public int offset;
            public double msPerBeat;

            public TimingPoint(string s) {
                var values = s.Split(new char[] { ',' });
                offset = int.Parse(values[0]);
                msPerBeat = double.Parse(values[1], CultureInfo.InvariantCulture);
            }
        }
        public class HitObject {
            public int column;
            public int start;
            public int end;
            public bool IsSingle {
                get => end == 0;
            }

            public HitObject(string s) {
                var values = s.Split(new char[] { ',', ':', '|' });
                column = (int)Math.Floor(int.Parse(values[0]) / 512f * Config.columns);
                start = int.Parse(values[2]);
                if (int.Parse(values[3]) == 128) end = int.Parse(values[5]);
                else end = 0;
            }
        }

        public List<TimingPoint> timingPoints = new List<TimingPoint>();
        public List<HitObject> hitObjects = new List<HitObject>();

        public Beatmap(string path) {
            string s = File.ReadAllText(path);
            Config.columns = byte.Parse(s.Substring(s.IndexOf("CircleSize:") + 11, 1));

            int startindex = s.IndexOf("[TimingPoints]") + 14;
            int endindex = s.IndexOf("[HitObjects]");

            string timingPointsString = s.Substring(startindex, endindex - startindex).Trim();
            string hitObjectsString = s.Substring(endindex + 12).Trim();

            foreach (var ho in hitObjectsString.Split('\n')) hitObjects.Add(new HitObject(ho));
            foreach (var tp in timingPointsString.Split('\n')) {
                var timingPoint = new TimingPoint(tp);
                if (timingPoint.msPerBeat >= 0) timingPoints.Add(timingPoint);
            }
        }
    }
}
