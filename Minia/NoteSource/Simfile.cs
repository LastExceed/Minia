using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minia {
    class Simfile {
        public Dictionary<string, string> properties = new Dictionary<string, string>();

        public Simfile (string path) {
            var lines = File.ReadAllLines(path);
            
            foreach (var line in lines) {
                if (line == "") continue;
                if (line.StartsWith("#")) {
                    var spl = line.Split(':');
                    properties.Add(spl[0], spl[1]);
                    continue;
                }
            }
        }
    }
}
