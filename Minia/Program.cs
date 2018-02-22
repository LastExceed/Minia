using NAudio;
using NAudio.Wave;
using System;
using System.Media;
using System.Threading.Tasks;

namespace Minia {
    class Program {
        [STAThread]
        static void Main(string[] args) {
            using (Game game = new Game()) {
                game.Run(0f);
            }
        }
    }
}
