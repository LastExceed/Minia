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
        Stopwatch sw = new Stopwatch();
        double frameTime = 0;
        double drawTime = 0;

        public Game() : base(300, 500, GraphicsMode.Default, "Minia") {
            
        }
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            VSync = VSyncMode.Off;
            CursorVisible = false;
            WindowState = WindowState.Fullscreen;
            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
            //GL.MatrixMode(MatrixMode.Color);
            GL.LoadMatrix(ref modelview);
            sw.Start();
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            var xx = sw.ElapsedTicks;
            switch (Config.screen) {
                case Screen.Stage:
                    Stage.Draw(Audio.sw.Elapsed.TotalMilliseconds);
                    break;
                case Screen.SongSelection:
                    SongSelection.Draw();
                    break;
            }
            var yy = sw.ElapsedTicks;

            SwapBuffers();
            var zz = sw.ElapsedTicks;
            frameTime = e.Time;
            drawTime = (yy - xx) * 1000 / (double)Stopwatch.Frequency;
        }
        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
            if (Config.screen == Screen.SongSelection) return;
            Console.CursorTop = 0;
            Console.CursorLeft = 0;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("frameTime  : " + frameTime);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("drawTime   : " + drawTime);
            Console.WriteLine("AudioDesync: " + Audio.Desync);
            Console.WriteLine("Score      : " + Score.Result);
            Console.WriteLine("Average    : " + Score.Average);
        }
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);
            if (Config.screen == Screen.Stage) {
                Stage.OnKey(e, true, Audio.sw.Elapsed.TotalMilliseconds);
            }
            else {
                SongSelection.OnKeyDown(e);
            }
        }
        protected override void OnKeyUp(KeyboardKeyEventArgs e) {
            base.OnKeyUp(e);
            if (Config.screen == Screen.Stage) {
                Stage.OnKey(e, false, Audio.sw.Elapsed.TotalMilliseconds);
            }
        }
        protected override void OnKeyPress(KeyPressEventArgs e) {
            base.OnKeyPress(e);
            if (Config.screen == Screen.SongSelection) {
                SongSelection.OnKeyPress(e);
            }
        }
    }
}
