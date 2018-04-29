using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using Minia.Judge;

namespace Minia {
    static class Stage {
        static ColumnData[] columns;
        public static Beatmap beatmap;
        static List<Key> keyLayout;

        public static float GetColumnStart(int column) {
            return ColumnWidth * column - Config.stageWidthScale;
        }
        public static float ColumnWidth {
            get => 2f / beatmap.columns * Config.stageWidthScale;
        }

        public static void Load(Beatmap beatmap) {
            columns = new ColumnData[beatmap.columns];
            for (int i = 0; i < beatmap.columns; i++) {
                columns[i] = new ColumnData {
                    notes = beatmap.hitObjects.FindAll(x => x.column == i)
                };
            }

            Judgement.Load(beatmap.columns);
            keyLayout = new List<Key>();
            switch (beatmap.columns) {
                case 4:
                    keyLayout.Add(Key.D);
                    keyLayout.Add(Key.F);
                    keyLayout.Add(Key.J);
                    keyLayout.Add(Key.K);
                    break;
                case 5:
                    keyLayout.Add(Key.D);
                    keyLayout.Add(Key.F);
                    keyLayout.Add(Key.Space);
                    keyLayout.Add(Key.J);
                    keyLayout.Add(Key.K);
                    break;
                case 6:
                    keyLayout.Add(Key.S);
                    keyLayout.Add(Key.D);
                    keyLayout.Add(Key.F);
                    keyLayout.Add(Key.J);
                    keyLayout.Add(Key.K);
                    keyLayout.Add(Key.L);
                    break;
                case 7:
                    keyLayout.Add(Key.S);
                    keyLayout.Add(Key.D);
                    keyLayout.Add(Key.F);
                    keyLayout.Add(Key.Space);
                    keyLayout.Add(Key.J);
                    keyLayout.Add(Key.K);
                    keyLayout.Add(Key.L);
                    break;
                default:
                    break;
            }
            //Audio.ResetAndSync();
            Stage.beatmap = beatmap;
        }

        public static void Draw(double time) {
            if (columns.Length % 2 != 0) {
                Shapes.Rectangle(-ColumnWidth / 2f, 1f, ColumnWidth / 2f, -1f, Color.Purple);
            }
            for (byte column = 0; column < beatmap.columns; column++) {
                var columnData = columns[column];
                for (int i = columnData.startPos; i < columnData.notes.Count; i++) {
                    var ho = columnData.notes[i];
                    if (ho.start > time + Config.scrollTime) break;
                    else if (ho.start < time - Config.hitWindow) {
                        columnData.startPos++;
                        if (Config.screen == Screen.Stage) {
                            Judgement.Judge(Config.hitWindow, time, column);
                        }
                    }
                    else {
                        float noteX = GetColumnStart(column) + ColumnWidth / 2f;
                        float noteY = (float)(2f / Config.scrollTime * (ho.start - time) - 1f);
                        Noteskin.DrawNote(noteX, noteY, Color.White);
                        if (!ho.IsSingle) Noteskin.DrawSlider(noteX, noteY, (float)(2f / Config.scrollTime * (ho.end - time) - 1f), Color.Red);
                    }
                }
                if (columnData.isHolding) {
                    if (columnData.holdEndTime < time - Config.hitWindow) {
                        //release missed
                    }
                    else {
                        Noteskin.DrawSlider(
                            GetColumnStart(column) + ColumnWidth / 2f,
                            -1f,
                            (float)(2f / Config.scrollTime * (columnData.holdEndTime - time) - 1f),
                            Color.Red
                        );
                    }
                }
            }
            Shapes.Rectangle(GetColumnStart(0), 1f, GetColumnStart(columns.Length - 1) + ColumnWidth, 1f - Config.topCover, Color.Black);
            Shapes.Rectangle(GetColumnStart(0), -1f, GetColumnStart(columns.Length - 1) + ColumnWidth, Config.bottomCover - 1f, Color.Black);
            Shapes.Line(GetColumnStart(0), -0.999f, -GetColumnStart(0), -0.999f, Color.White);
        }

        public static void OnKey(KeyboardKeyEventArgs e, bool down, double time) {
            if (e.IsRepeat) return;
            if (down && e.Key == Key.Escape) {
                Config.screen = Screen.SongSelection;
                return;
            }
            if (!keyLayout.Contains(e.Key)) return;
            byte column = (byte)keyLayout.IndexOf(e.Key);
            var columnData = columns[column];
            double offset;
            if (down) {
                Audio.hit.CurrentTime = new TimeSpan(0);
                if (columnData.startPos >= columnData.notes.Count) return;
                offset = time - columnData.notes[columnData.startPos].start;
                if (offset < -Config.hitWindow) return;
                if (!columnData.notes[columnData.startPos].IsSingle) {
                    columnData.isHolding = true;
                    columnData.holdEndTime = columnData.notes[columnData.startPos].end;
                }
                columnData.startPos++;
            }
            else {
                if (!columnData.isHolding) return;
                columnData.isHolding = false;
                offset = time - columnData.holdEndTime;
                if (offset < -Config.hitWindow) offset = -Config.hitWindow;
            }
            Judgement.Judge(offset, time, column);
        }
    }
}
