using System;

namespace Minia {
    static class Program {
        public static Game game = new Game();
        [STAThread]
        static void Main(string[] args) {
            game.Run(60f);
            game.Dispose();
        }
    }
}
