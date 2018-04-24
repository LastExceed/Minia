using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Minia {
    class Beatmap {
        public class TimingPoint {
            public int offset;
            public double msPerBeat;

            public TimingPoint(string s) {
                var values = s.Split(new char[] { ',' });
                offset = (int)double.Parse(values[0]);
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

            public HitObject(string line, byte columns) {
                var values = line.Split(new char[] { ',', ':', '|' });
                column = (int)Math.Floor(int.Parse(values[0]) / 512f * columns);
                start = int.Parse(values[2]);
                if (int.Parse(values[3]) == 128) end = int.Parse(values[5]);
                else end = 0;
            }
        }

        public Dictionary<string, string> properties = new Dictionary<string, string>();
        public List<TimingPoint> timingPoints = new List<TimingPoint>();
        public List<HitObject> hitObjects = new List<HitObject>();
        public byte columns;
        public byte mode;

        public Beatmap(string path) {
            var lines = File.ReadAllLines(path);

            string section = null;
            foreach (var line in lines) {
                if (line == "" || line.StartsWith("//")) continue;
                if (line.StartsWith("[")) {
                    section = line;
                    continue;
                }
                if (section == "[TimingPoints]") {
                    var timingPoint = new TimingPoint(line);
                    if (timingPoint.msPerBeat >= 0) timingPoints.Add(timingPoint);
                }
                else if (section == "[HitObjects]") {
                    hitObjects.Add(new HitObject(line, columns));
                }
                else {
                    var kvp = line.Split(':');
                    if (kvp.Length < 2) continue;
                    if (kvp[1].StartsWith(" ")) {
                        kvp[1] = kvp[1].Substring(1);
                    }
                    kvp[0] = kvp[0].ToLower();
                    if (kvp[0] == "mode") {
                        mode = byte.Parse(kvp[1]);
                    }
                    if (kvp[0] == "circlesize" && mode == 3) columns = byte.Parse(kvp[1]);
                    properties.Add(kvp[0], kvp[1]);
                }
            }
        }
    }
}
