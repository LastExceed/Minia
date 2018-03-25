using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minia {
    static class SongSelection {
        static string[] mapsetDirectories;
        static string[] searchResults;
        static string[] diffFiles;
        static List<Beatmap> diffs;
        static int selectedMapset;
        static int selectedDiff;
        static string currentAudioFile;
        static string searchQuery = "";

        static SongSelection() {
            mapsetDirectories = searchResults = Directory.GetDirectories(Config.songsDirectory);
            SelectMapset(1);
        }

        public static void Draw() {
            for (int i = -10; i <= 10; i++) {
                if (selectedMapset + i < 0 || selectedMapset + i >= searchResults.Length) continue;
                if (i == 0) {
                    Shapes.Rectangle(-1f, 0.05f, 0f, -0.05f, Color.Gray);
                }
                var folderName = searchResults[selectedMapset + i].Substring(Config.songsDirectory.Length);
                if (folderName.Contains(" ") && int.TryParse(folderName.Substring(0, folderName.IndexOf(" ")), out int id)) folderName = folderName.Substring(folderName.IndexOf(" ") + 1);
                Shapes.Text(folderName, -1f, i * -0.1f, 0.05f, Color.Yellow);
            }
            for (int i = 0; i < diffFiles.Length; i++) {
                if (i == selectedDiff) {
                    Shapes.Rectangle(0f, 1f - i * 0.1f + 0.05f, 1f, 1f - (i + 1) * 0.1f + 0.05f, Color.Gray);
                }
                string diffName;
                Color color;
                if (diffs?[i] == null || !diffs[i].properties.ContainsKey("version")) {
                    diffName = diffFiles[i];
                    color = Color.Yellow;
                }
                else {
                    diffName = diffs[i].properties["version"];
                    var r = diffs[i].mode;
                    color = diffs[i].mode == 3 ? Color.Green : Color.Red;
                }
                Shapes.Text(diffName, 0f, 1f - 0.1f * i, 0.05f, color);
            }
        }

        private static void SelectMapset(int index) {
            selectedMapset = index;
            diffs = new List<Beatmap>();
            if (index == -1) {
                diffFiles = new string[0];
                SelectDiff(-1);
                return;
            }
            diffFiles = Directory.GetFiles(searchResults[selectedMapset], "*.osu");
            for (int i = 0; i < diffFiles.Length; i++) {
                diffs.Add(new Beatmap(diffFiles[i]));
            }
            if (diffs.Count == 0) {
                //no diffs
                return;
            }
            SelectDiff(0);
        }
        private static void SelectDiff(int index) {
            selectedDiff = index;
            var audioFile = selectedDiff == -1 ? null : searchResults[selectedMapset] + @"\" + diffs[index].properties["audiofilename"];
            if (audioFile != currentAudioFile) {//audioFile changed
                try {
                    Audio.SetMusic(audioFile);
                }
                catch {
                    Console.Title = "error";
                }
                currentAudioFile = audioFile;
            }
        }

        private static void OnSearchQueryChanged(char c) {
            string[] source;
            var previous = selectedMapset == -1 ? null : searchResults[selectedMapset];
            if (c == '\b') {
                searchQuery = searchQuery.Remove(searchQuery.Length - 1);
                source = mapsetDirectories;
            }
            else {
                searchQuery += c;
                source = searchResults;
            }
            searchResults = source.Where(path => path.ToLower().Contains(searchQuery)).ToArray();
            Console.Clear();
            Console.WriteLine(searchQuery);
            for (int i = 0; i < searchResults.Length; i++) {
                if (searchResults[i] == previous) {
                    selectedMapset = i;
                    return;
                }
            }
            SelectMapset(searchResults.Length == 0 ? -1 : 0);
        }

        public static void OnKeyDown(KeyboardKeyEventArgs e) {
            switch (e.Key) {
                case Key.Left:
                    SelectMapset(selectedMapset - 1);
                    break;
                case Key.Right:
                    SelectMapset(selectedMapset + 1);
                    break;
                case Key.Up:
                    SelectDiff(selectedDiff - 1);
                    break;
                case Key.Down:
                    SelectDiff(selectedDiff + 1);
                    break;
                case Key.Enter:
                    Stage.Load(diffs[selectedDiff]);
                    break;
                case Key.BackSpace when searchQuery.Length != 0:
                    OnSearchQueryChanged('\b');
                    break;
                case Key.Escape:
                    Environment.Exit(0);
                    break;
                default:
                    break;
            }
        }

        public static void OnKeyPress(KeyPressEventArgs e) {
            OnSearchQueryChanged(e.KeyChar);
        }
    }
}
