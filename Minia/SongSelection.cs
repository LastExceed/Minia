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
        static Random rnd = new Random();

        static SongSelection() {
            mapsetDirectories = searchResults = Directory.GetDirectories(Config.songsDirectory);
            SelectMapset(rnd.Next(searchResults.Length - 1));
        }

        public static void Draw() {
            Shapes.Rectangle(-1f, 0.05f, 0.33f, -0.1f, Color.Gray);
            for (int i = -5; i <= 5; i++) {
                if (selectedMapset + i < 0 || selectedMapset + i >= searchResults.Length) continue;
                var folderName = searchResults[selectedMapset + i].Substring(Config.songsDirectory.Length);
                if (folderName.Contains(" ") && int.TryParse(folderName.Substring(0, folderName.IndexOf(" ")), out int id)) folderName = folderName.Substring(folderName.IndexOf(" ") + 1);
                string artist = null;
                if (folderName.Contains(" - ")) {
                    var index = folderName.IndexOf(" - ");
                    artist = folderName.Remove(index);
                    folderName = folderName.Substring(index + 3);
                }
                Shapes.Text(folderName, -1f, i * -0.2f, 0.1f, Color.Yellow);
                if (artist != null) {
                    Shapes.Text(artist, -1f, i * -0.2f - 0.075f, 0.05f, Color.LightGray);
                }
            }
            for (int i = 0; i < diffFiles.Length; i++) {
                if (i == selectedDiff) {
                    Shapes.Rectangle(0.33f, 1f - i * 0.066f, 1f, 1f - (i + 1) * 0.066f, Color.Gray);
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
                Shapes.Text(diffName, 0.33f, 1f - 0.066f * i - 0.033f, 0.05f, color);
            }
            Shapes.Text(searchQuery, 0f, -0.9f, 0.1f, Color.Cyan);
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
            Stage.Load(diffs[index]);
            if (audioFile != currentAudioFile) {//audioFile changed
                currentAudioFile = audioFile;
                if (audioFile == null) return;
                Audio.SetMusic(audioFile);
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
                searchQuery += c.ToString().ToLower();
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
            Audio.hit.CurrentTime = new TimeSpan(0);
            switch (e.Key) {
                case Key.Left:
                    if (selectedMapset > 0) {
                        SelectMapset(selectedMapset - 1);
                    }
                    break;
                case Key.Right:
                    if (selectedMapset < searchResults.Length - 1) {
                        SelectMapset(selectedMapset + 1);
                    }
                    break;
                case Key.Up:
                    if (selectedDiff > 0) {
                        SelectDiff(selectedDiff - 1);
                    }
                    break;
                case Key.Down:
                    if (selectedDiff < diffs.Count - 1) {
                        SelectDiff(selectedDiff + 1);
                    }
                    break;
                case Key.Enter:
                    if (Audio.musicCompatible) {
                        Audio.ResetAndSync();

                        Config.screen = Screen.Stage;
                        Stage.Load(diffs[selectedDiff]);
                    }
                    else {
                        Audio.miss.CurrentTime = new TimeSpan(0);
                    }
                    break;
                case Key.BackSpace when searchQuery.Length != 0:
                    OnSearchQueryChanged('\b');
                    break;
                case Key.F2:
                    SelectMapset(rnd.Next(searchResults.Length - 1));
                    break;
                case Key.Escape:
                    if(searchQuery != "") {
                        searchQuery = " ";
                        OnSearchQueryChanged('\b');
                    }
                    else {
                        Program.game.Close();
                    }
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
