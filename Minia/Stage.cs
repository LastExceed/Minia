using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Minia {
    static class Stage {
        static ColumnData[] columns;
        static Beatmap beatmap;
        static List<Key> keyLayout = new List<Key>();

        public static float GetColumnStart(int column) {
            return ColumnWidth * column - Config.stageWidthScale;
        }
        public static float ColumnWidth {
            get => 2f / beatmap.columns * Config.stageWidthScale;
        }

        public static void Load(Beatmap beatmap) {
            Stage.beatmap = beatmap;

            columns = new ColumnData[beatmap.columns];
            for (int i = 0; i < beatmap.columns; i++) {
                columns[i] = new ColumnData {
                    notes = beatmap.hitObjects.FindAll(x => x.column == i)
                };
            }

            Judgement.Load(beatmap.columns);

            switch (beatmap.columns) {
                case 4:
                    keyLayout.Add(Key.D);
                    keyLayout.Add(Key.F);
                    keyLayout.Add(Key.J);
                    keyLayout.Add(Key.K);
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
            Audio.ResetAndSync();
            Config.screen = Screen.Stage;
        }

        public static void Draw(double time) {
            for (byte column = 0; column < beatmap.columns; column++) {
                var columnData = columns[column];
                for (int i = columnData.startPos; i < columnData.notes.Count; i++) {
                    Judgement.DrawJudgeHighlighting(column, time);
                    var ho = columnData.notes[i];
                    if (ho.start > time + Config.scrollTime) break;
                    else if (ho.start < time - Config.hitWindow) {
                        columnData.startPos++;
                        Judgement.Judge(Config.hitWindow, time, column);
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
                                            Color.Red);
                    }
                }
            }
            Judgement.DrawJudgeMeter();
        }

        public static void OnKey(Key key, bool down, double time) {
            if (down && key == Key.Escape) {
                Config.screen = Screen.SongSelection;
                return;
            }
            if (!keyLayout.Contains(key)) return;
            byte column = (byte)keyLayout.IndexOf(key);
            var columnData = columns[column];
            double offset;
            if (down) {
                Audio.hit.Position = 0;
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
