using NAudio.Wave;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Threading.Tasks;

namespace Minia {
    class Game : GameWindow {
        Beatmap beatmap = new Beatmap("b.osu");
        //AsioOut asioOut;
        WaveChannel32 hitsound = new WaveChannel32(new WaveFileReader(Properties.Resources.drum_hitfinish)) {//new Mp3FileReader("boss.mp3")
            PadWithZeroes = true
        };
        WaveChannel32 music = new WaveChannel32(new Mp3FileReader("audio.mp3")) {//new Mp3FileReader("boss.mp3")
            PadWithZeroes = true
        };

        public Game() : base(800, 600, GraphicsMode.Default, "Minia") {
            VSync = VSyncMode.On;
            CursorVisible = false;
            WindowState = WindowState.Fullscreen;
            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            beatmap.hitObjects.Reverse();
            var asioDrivers = AsioOut.GetDriverNames();
            if (asioDrivers.Length == 0) {
                Console.WriteLine("please install http://www.asio4all.org/");
                Console.WriteLine("press any key to exit");
                Console.ReadKey();
                return;
            }

            var asioOutMusic = new AsioOut(asioDrivers[0]);
            asioOutMusic.Init(music);
            asioOutMusic.Play();
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            int time = (int)music.CurrentTime.TotalMilliseconds;
            for (int i = beatmap.hitObjects.Count - 1; i >= 0; i--) {
                var ho = beatmap.hitObjects[i];
                if (ho.start < time - 100) {
                    beatmap.hitObjects.RemoveAt(i);
                    //miss
                }
                if (ho.start > time + 1000) break;
                float noteX = (float)ho.column / 2f - 1f;
                float noteY = 0.002f * (ho.start - time) - 1f;
                Line(-noteX, noteY, -(noteX + 0.5f), noteY);//still need to figure why columns are flipped
            }

            SwapBuffers();
            //Console.WriteLine(e.Time*1000);
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
            int col;
            switch (e.Key) {
                case Key.D:
                    col = 0;
                    break;
                case Key.F:
                    col = 1;
                    break;
                case Key.J:
                    col = 2;
                    break;
                case Key.K:
                    col = 3;
                    break;
                default:
                    return;
            }
            int time = (int)music.CurrentTime.TotalMilliseconds;
            for (int i = beatmap.hitObjects.Count - 1; i >= 0; i--) {
                var ho = beatmap.hitObjects[i];
                if (ho.start > time + 100) break;
                if (ho.column == col) {
                    beatmap.hitObjects.RemoveAt(i);
                    //hit
                    break;
                }
            }
            hitsound.Position = 0;
        }

        private void Line(float x1, float y1, float x2, float y2) {
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(x1, y1);
            GL.Vertex2(x2, y2);
            GL.End();
        }
    }
}
