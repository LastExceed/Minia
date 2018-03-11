using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Minia {
    class Game : GameWindow {
        Beatmap beatmap;
        ColumnData[] columns;
        Stopwatch sw = new Stopwatch();
        double time = 0f;//in ms
        double frameTime = 0;
        double drawTime = 0;
        List<Key> keyLayout = new List<Key>();

        public Game(string beatmapFile, string audioFile) : base(300, 500, GraphicsMode.Default, "Minia") {
            beatmap = new Beatmap(beatmapFile);
            Audio.Init(audioFile);
        }
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            VSync = VSyncMode.Off;
            CursorVisible = false;
            //WindowState = WindowState.Fullscreen;
            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            columns = new ColumnData[Config.columns];
            for (int i = 0; i < Config.columns; i++) {
                columns[i] = new ColumnData {
                    notes = beatmap.hitObjects.FindAll(x => x.column == i)
                };
            }

            switch (Config.columns) {
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

            Audio.StartAndSync();
            time = 0;
            sw.Start();
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            time += e.Time*1000;
            GL.Clear(ClearBufferMask.ColorBufferBit);

            var xx = sw.ElapsedTicks;
            for (byte column = 0; column < Config.columns; column++) {
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
                        float noteX = Config.GetColumnStart(column) - Config.ColumnWidth / 2f;
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
                        Noteskin.DrawSlider(Config.GetColumnStart(column) - Config.ColumnWidth / 2f, -1f, (float)(2f / Config.scrollTime * (columnData.holdEndTime - time) - 1f), Color.Red);
                    }
                }
            }
            Judgement.DrawJudgeMeter();
            var yy = sw.ElapsedTicks;

            SwapBuffers();
            var zz = sw.ElapsedTicks;
            frameTime = e.Time;
            drawTime = (yy - xx) * 1000 / (double)Stopwatch.Frequency;
        }
        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
            Console.CursorTop = 0;
            Console.CursorLeft = 0;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("frameTime  : " + frameTime);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("drawTime   : " + drawTime);
            Console.WriteLine("AudioDesync: " + Audio.GetDesync(time));
            Console.WriteLine("Score      : " + Score.Result);
            Console.WriteLine("Average    : " + Score.Average);
        }
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape) Environment.Exit(0);
            if (e.IsRepeat || !keyLayout.Contains(e.Key)) return;
            byte column = (byte)keyLayout.IndexOf(e.Key);
            Audio.Play("hit");
            var columnData = columns[column];
            var offset = time - columnData.notes[columnData.startPos].start;
            if (offset < -Config.hitWindow) return;
            if (!columnData.notes[columnData.startPos].IsSingle) {
                columnData.isHolding = true;
                columnData.holdEndTime = columnData.notes[columnData.startPos].end;
            }
            columnData.startPos++;
            Judgement.Judge(offset, time, column);
        }
        protected override void OnKeyUp(KeyboardKeyEventArgs e) {
            base.OnKeyUp(e);
            if (!keyLayout.Contains(e.Key)) return;
            byte column = (byte)keyLayout.IndexOf(e.Key);
            var columnData = columns[column];
            if (!columnData.isHolding) return;
            columnData.isHolding = false;
            var offset = time - columnData.holdEndTime;
            if (offset < -Config.hitWindow) offset = -Config.hitWindow;
            Judgement.Judge(offset, time, column);
        }
    }
}
