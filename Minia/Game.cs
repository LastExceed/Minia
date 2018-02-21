using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Minia {
    class Game : GameWindow {
        Stopwatch sw = new Stopwatch();
        Beatmap beatmap = new Beatmap("b.osu");
        int startPos = 0;
        static SoundPlayer player = new SoundPlayer(Properties.Resources.drum_hitfinish);
        public Game() : base(800, 600, GraphicsMode.Default, "Minia"){
            VSync = VSyncMode.Off;
            CursorVisible = false;
            //WindowState = WindowState.Fullscreen;
            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            beatmap.hitObjects.Reverse();
            sw.Start();
        }
        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            int time = (int)sw.ElapsedMilliseconds;
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
            
            
            //var height = (sw.ElapsedMilliseconds % 2000 - 1000) * -0.001f;
            //Line(-1, height, 1, height);

            //GL.Begin(PrimitiveType.Triangles);
            //GL.Color3(Color.Red);
            //GL.Vertex2(-0.5f, -0.5f);
            //GL.Color3(Color.Green);
            //GL.Vertex2(0.5f, -0.5f);
            //GL.Color3(Color.Blue);
            //GL.Vertex2(0f, 0.5f);
            //GL.End();
            SwapBuffers();
            //Console.WriteLine(e.Time*1000);
            //Console.WriteLine(RenderFrequency);
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
            int time = (int)sw.ElapsedMilliseconds;
            for (int i = beatmap.hitObjects.Count - 1; i >= 0; i--) {
                var ho = beatmap.hitObjects[i];
                if (ho.start > time + 100) break;
                if (ho.column == col) {
                    beatmap.hitObjects.RemoveAt(i);
                    //hit
                    break;
                }
            }

            player.Play();
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
        }

        private void Line(float x1, float y1, float x2, float y2) {
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(x1, y1);
            GL.Vertex2(x2, y2);
            GL.End();
        }
    }
}
