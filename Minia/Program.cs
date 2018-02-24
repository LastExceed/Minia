using System;

namespace Minia {
    class Program {
        [STAThread]
        static void Main(string[] args) {
            Console.ReadLine();
            using (Game game = new Game()) {
                game.Run(10f);
            };
        }
    }
}
