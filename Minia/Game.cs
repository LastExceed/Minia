using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Diagnostics;
using System.Drawing;

namespace Minia {
    class Game : GameWindow {
        Beatmap beatmap = new Beatmap("b.osu");
        ColumnData[] columns = new ColumnData[Config.columns];
        Stopwatch sw = new Stopwatch();
        double time = 0f;//in ms
        double frameTime = 0;
        double drawTime = 0;

        public Game() : base(300, 500, GraphicsMode.Default, "Minia") {
            
        }
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            VSync = VSyncMode.Off;
            CursorVisible = false;
            //WindowState = WindowState.Fullscreen;
            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            for (int i = 0; i < Config.columns; i++) {
                columns[i] = new ColumnData {
                    notes = beatmap.hitObjects.FindAll(x => x.column == i)
                };
            }

            Audio.Init();
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
                        float noteX = -column / 2f + 0.75f;
                        float noteY = (float)(2f / Config.scrollTime * (ho.start - time) - 1f);
                        float columnWidth = 0.5f;
                        Noteskin.DrawNote(noteX, noteY, columnWidth, Color.White);
                        if (!ho.IsSingle) Noteskin.DrawSlider(noteX, noteY, (float)(2f / Config.scrollTime * (ho.end - time) - 1f), columnWidth, Color.Red);
                    }
                }
                if (columnData.isHolding) {
                    if (columnData.holdEndTime < time - Config.hitWindow) {
                        //release missed
                    }
                    else {
                        Noteskin.DrawSlider(-column / 2f + 0.75f, -1f, (float)(2f / Config.scrollTime * (columnData.holdEndTime - time) - 1f), 0.5f, Color.Red);
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
            Console.WriteLine("frameTime  : " + frameTime);
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
            if (!e.IsRepeat) {
                byte column;
                switch (e.Key) {
                    case Key.D:
                        column = 0;
                        break;
                    case Key.F:
                        column = 1;
                        break;
                    case Key.J:
                        column = 2;
                        break;
                    case Key.K:
                        column = 3;
                        break;
                    case Key.Escape:
                        Environment.Exit(0);
                        return;
                    default:
                        return;
                }
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
        }
        protected override void OnKeyUp(KeyboardKeyEventArgs e) {
            base.OnKeyUp(e);
            byte column;
            switch (e.Key) {
                case Key.D:
                    column = 0;
                    break;
                case Key.F:
                    column = 1;
                    break;
                case Key.J:
                    column = 2;
                    break;
                case Key.K:
                    column = 3;
                    break;
                default:
                    return;
            }
            var columnData = columns[column];
            if (!columnData.isHolding) return;
            columnData.isHolding = false;
            var offset = time - columnData.holdEndTime;
            if (offset < -Config.hitWindow) offset = -Config.hitWindow;
            Judgement.Judge(offset, time, column);
        }
    }
}
