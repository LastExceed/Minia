using NAudio.Wave;
using System;
using System.IO;

namespace Minia {
    static class SongSelection {
        static int songIndex = 1621;
        static string[] songDirectories;
        static AsioOut asioOut;
        static AudioFileReader audioFileReader;
        static string diff;
        static string audioFile;

        public static void Show() {
            Console.BufferHeight = 25;
            songDirectories = Directory.GetDirectories(Config.songsDirectory);
            Refresh();
            while (true) OnKey(Console.ReadKey());
        }

        private static void Refresh() {
            Console.Clear();
            for (int i = -12; i < 13; i++) {
                if (songIndex + i < 0) {
                    Console.WriteLine();
                    continue;
                }
                if (i == 0) {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }
                else {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                var folderName = songDirectories[songIndex + i].Substring(Config.songsDirectory.Length + 1);
                if (folderName.Contains(" ") && int.TryParse(folderName.Substring(0, folderName.IndexOf(" ")), out int id)) folderName = folderName.Substring(folderName.IndexOf(" ")+ 1);
                Console.Write(folderName);
                if (i != 12) Console.WriteLine();
            }
            var diffs = Directory.GetFiles(songDirectories[songIndex], "*.osu");
            if (asioOut != null) {
                asioOut.Stop();
                asioOut.Dispose();
                asioOut = null;
                audioFileReader.Dispose();
            }
            if (diffs.Length == 0) return;

            string line;
            diff = diffs[0];
            using (var reader = File.OpenText(diff)) {
                do {
                    line = reader.ReadLine();
                } while (!line.StartsWith("AudioFile"));
            }
            audioFile = songDirectories[songIndex] + @"\" + line.Substring(15);
            audioFileReader = new AudioFileReader(audioFile) {
                Volume = 0.2f
            };
            asioOut = new AsioOut();
            asioOut.Init(audioFileReader);
            asioOut.Play();
            audioFileReader.Skip(10);
        }

        private static void OnKey(ConsoleKeyInfo keyInfo) {
            switch (keyInfo.Key) {
                case ConsoleKey.RightArrow:
                    songIndex++;
                    Refresh();
                    break;
                case ConsoleKey.LeftArrow:
                    if (songIndex != 0) {
                        songIndex--;
                        Refresh();
                    }
                    break;
                case ConsoleKey.Enter:
                    Run();
                    break;
                case ConsoleKey.Backspace:
                    break;
                default:
                    break;
            }
        }

        public static void Run() {
            Console.Clear();
            using (Game game = new Game(diff, audioFile)) {
                game.Run(60f);
            };
        }
    }
}
