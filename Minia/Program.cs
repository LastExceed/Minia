using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minia {
    class Program {
        static void Main(string[] args) {
            using (Game game = new Game()) {
                game.Run(0f);
            }
        }
    }
}
