using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter {
    class Program {
        const string source = @"D:\osu!\Songs";
        const string destination = "";
        const string beatmapHeader = "osu file format v";

        static void Main(string[] args) {
            var directories = Directory.GetDirectories(source);
            for (int mapIndex = 0; mapIndex < directories.Length; mapIndex++) {
                var diffs = Directory.GetFiles(directories[mapIndex], "*.osu");
                var dir = directories[mapIndex];
                var mapName = dir.Substring(dir.LastIndexOf(@"\") + 1);
                Console.WriteLine(mapName);
                Console.WriteLine("diffs found: " + diffs.Length);
                int maniadiffs = 0;
                for (int diffIndex = 0; diffIndex < diffs.Length; diffIndex++) {
                    var diffName = diffs[diffIndex].Substring(diffs[diffIndex].LastIndexOf(@"\") + 1);
                    var lines = File.ReadAllLines(diffs[diffIndex]);
                    if (!lines[0].StartsWith(beatmapHeader) || !int.TryParse(lines[0].Substring(beatmapHeader.Length), out int version)) {
                        Console.WriteLine(diffIndex + " missing header: " + diffName);
                        continue;
                    }
                    var modeLine = lines.FirstOrDefault(line => line.StartsWith("Mode:"));
                    if (!int.TryParse(modeLine?.Substring(modeLine.IndexOf(":") + 1), out int mode)) {
                        Console.WriteLine(diffIndex + " no mode: " + diffName);
                        continue;
                    }
                    if (mode != 3) {
                        Console.WriteLine(diffIndex + " other");
                        continue;
                    }
                    Console.WriteLine(diffIndex + " mania");
                    maniadiffs++;
                }
                Console.WriteLine("maniadiffs: " + maniadiffs);
                Console.ReadLine();
            }
        }
    }
}
