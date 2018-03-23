using System;

namespace Minia {
    class Program {
        [STAThread]
        static void Main(string[] args) {
            //SongSelection.Run();
            //SongSelection.Show();
            using (Game game = new Game()) {
                game.Run(60f);
            };
        }
    }
}
