using NAudio.Wave;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace Minia {
    class Game : GameWindow {
        Beatmap beatmap = new Beatmap("b.osu");
        WaveChannel32 hitsound = new WaveChannel32(new WaveFileReader(Properties.Resources.drum_hitfinish), 0.15f, 0f) {//new Mp3FileReader("boss.mp3")
            PadWithZeroes = true
        };
        WaveChannel32 music = new WaveChannel32(new Mp3FileReader("audio.mp3"), 0.15f, 0f) {//new Mp3FileReader("boss.mp3")
            PadWithZeroes = true
        };
        List<Beatmap.HitObject>[] notes = new List<Beatmap.HitObject>[4];
        int[] startPos = new int[4];
        Stopwatch sw = new Stopwatch();

        public Game() : base(200, 600, GraphicsMode.Default, "Minia") {
            VSync = VSyncMode.On;
            CursorVisible = false;
            WindowState = WindowState.Fullscreen;
            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            for (int i = 0; i < 4; i++) notes[i] = beatmap.hitObjects.FindAll(x => x.column == i);


            var asioDrivers = AsioOut.GetDriverNames();
            if (asioDrivers.Length == 0) {
                Console.WriteLine("please install http://www.asio4all.org/");
                Console.WriteLine("press any key to exit");
                Console.ReadKey();
                return;
            }

            var asioOut = new AsioOut(asioDrivers[0]);
            asioOut.Init(new WaveMixerStream32(new WaveStream[2] { music, hitsound }, false));
            asioOut.Play();
            sw.Start();
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            var xx = sw.ElapsedTicks;

            int time = (int)music.CurrentTime.TotalMilliseconds;
            int upperLimit = time + 1000;
            int lowerLimit = time - 100;
            int columns = notes.Length;
            for (int column = 0; column < columns; column++) {
                var columnNotes = notes[column];
                var columnNotesCount = columnNotes.Count;
                for (int i = startPos[column]; i < columnNotesCount; i++) {
                    var ho = columnNotes[i];
                    if (ho.start > upperLimit) break;
                    if (ho.start < lowerLimit) {
                        startPos[column]++;
                        //miss
                    }
                    else {
                        float noteX = ho.column / 2f - 1f;
                        float noteY = 0.002f * (ho.start - time) - 1f;
                        Line(noteX, noteY, noteX + 0.5f, noteY);
                        if (!ho.IsSingle) {
                            float lnX = noteX + 0.25f;
                            float lnY = 0.002f * (ho.end - time) - 1f;
                            Line(lnX, noteY, lnX, lnY);
                        }
                    }
                }
            }

            var yy = sw.ElapsedTicks;
            SwapBuffers();
            Console.Write(e.Time*1000);
            Console.CursorLeft = 50;
            Console.WriteLine((yy-xx)*1000 / (double)Stopwatch.Frequency);
            //Console.WriteLine(RenderFrequency);
        }
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);
            if (!e.IsRepeat) Task.Run(() => OnKeyDownAsync(e));
        }
        private void OnKeyDownAsync(KeyboardKeyEventArgs e) {
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
            if (notes[column][startPos[column]].start < music.CurrentTime.TotalMilliseconds + 500) {
                startPos[column]++;
                //hit
            }
            hitsound.Position = 0;
        }

        private void Line(float x1, float y1, float x2, float y2) {
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(-x1 * 0.25f, y1);
            GL.Vertex2(-x2 * 0.25f, y2);//still need to figure out why columns are flipped
            GL.End();
        }
    }
}
