using NAudio.Wave;
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
        Beatmap beatmap = new Beatmap("b.osu");
        WaveChannel32 miss = new WaveChannel32(new WaveFileReader(Properties.Resources.miss), 1f, 0f) {//new Mp3FileReader("boss.mp3")
            PadWithZeroes = true
        };
        WaveChannel32 hitsound = new WaveChannel32(new WaveFileReader(Properties.Resources.hitsound), 0.15f, 0f) {//new Mp3FileReader("boss.mp3")
            PadWithZeroes = true
        };
        WaveChannel32 music = new WaveChannel32(new Mp3FileReader("audio.mp3"), 0.15f, 0f) {//new Mp3FileReader("boss.mp3")
            PadWithZeroes = true
        };

        List<Beatmap.HitObject>[] notes = new List<Beatmap.HitObject>[4];
        int[] startPos = new int[4];
        double[] judgeTime = new double[4];
        Color[] judgeColor = new Color[4];

        Stopwatch sw = new Stopwatch();
        short scrollTime = 750;
        double time = 0f;//in ms
        double hitwindow = 100f;
        double frameTime = 0;
        double drawTime = 0;

        double hitOffsetStack = 0;
        int notesPassed = 0;

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

            for (int i = 0; i < 4; i++) notes[i] = beatmap.hitObjects.FindAll(x => x.column == i);

            var asioDrivers = AsioOut.GetDriverNames();
            if (asioDrivers.Length == 0) {
                Console.WriteLine("please install http://www.asio4all.org/");
                Console.WriteLine("press any key to exit");
                Console.ReadKey();
                Environment.Exit(0);
            }
            var asioOut = new AsioOut(asioDrivers[0]);
            //asioOut.ShowControlPanel();
            asioOut.Init(new WaveMixerStream32(new WaveStream[3] { music, hitsound, miss }, false));
            asioOut.Play();
            while (music.CurrentTime.TotalSeconds < 0.1f) ;//wait for audio playback to become fluent
            music.Position = 0;
            time = 0;
            sw.Start();
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            time += e.Time*1000;
            GL.Clear(ClearBufferMask.ColorBufferBit);

            var xx = sw.ElapsedTicks;
            for (int column = 0; column < notes.Length; column++) {
                for (int i = startPos[column]; i < notes[column].Count; i++) {
                    if (judgeTime[column] > time) Draw.Rectangle(column / 2f - 1f, 1f, column / 2f - 0.5f, -1f, judgeColor[column]);
                    var ho = notes[column][i];
                    if (ho.start > time + scrollTime) break;
                    if (ho.start < time - hitwindow) {
                        startPos[column]++;
                        hitOffsetStack += 100;
                        notesPassed++;
                        miss.Position = 0;
                    }
                    else {
                        float noteX = column / 2f - 0.75f;
                        float noteY = (float)(2f / scrollTime * (ho.start - time) - 1f);
                        float columnWidth = 0.5f;
                        Note.DrawNote(noteX, noteY, columnWidth, Color.White);
                        if (!ho.IsSingle) Note.DrawSlider(noteX, noteY, (float)(2f / scrollTime * (ho.end - time) - 1f), columnWidth, Color.Red);
                    }
                }
            }
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
            Console.WriteLine(frameTime);
            Console.WriteLine(drawTime);
            Console.WriteLine(music.CurrentTime.TotalMilliseconds - time);//audio desync
            Console.WriteLine(hitOffsetStack / notesPassed);
        }
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);
            if (!e.IsRepeat) {
                int column;
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
                hitsound.Position = 0;
                var nextNoteOffset = Math.Abs(notes[column][startPos[column]].start - time);
                if (nextNoteOffset > 100) return;
                startPos[column]++;
                judgeTime[column] = time + 33f;
                if (nextNoteOffset < 33.3333333f) judgeTime[column] = 0;
                else if (nextNoteOffset < 66.66666666f) judgeColor[column] = Color.Green;
                else if (nextNoteOffset <= 100f) judgeColor[column] = Color.Yellow;
                else judgeColor[column] = Color.Red;
                hitOffsetStack += nextNoteOffset;
                notesPassed++;
            }
        }
    }
}
