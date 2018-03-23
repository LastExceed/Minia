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
            //WindowState = WindowState.Fullscreen;
            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
            //GL.MatrixMode(MatrixMode.Color);
            GL.LoadMatrix(ref modelview);

            //Audio.ResetAndSync();
            sw.Start();
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            Audio.time += e.Time*1000;
            GL.Clear(ClearBufferMask.ColorBufferBit);

            var xx = sw.ElapsedTicks;
            switch (Config.screen) {
                case Screen.Stage:
                    Stage.Draw(Audio.time);
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
            Console.CursorTop = 0;
            Console.CursorLeft = 0;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("frameTime  : " + frameTime);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("drawTime   : " + drawTime);
            //Console.WriteLine("AudioDesync: " + Audio.GetDesync(Audio.time));
            Console.WriteLine("Score      : " + Score.Result);
            Console.WriteLine("Average    : " + Score.Average);
        }
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);
            if (e.IsRepeat) return;
            switch (Config.screen) {
                case Screen.Stage:
                    Stage.OnKey(e.Key, true, Audio.time);
                    break;
                case Screen.SongSelection:
                    SongSelection.OnKey(e.Key, true);
                    break;
            }
        }
        protected override void OnKeyUp(KeyboardKeyEventArgs e) {
            base.OnKeyUp(e);
            switch (Config.screen) {
                case Screen.Stage:
                    Stage.OnKey(e.Key, false, Audio.time);
                    break;
                case Screen.SongSelection:
                    SongSelection.OnKey(e.Key, false);
                    break;
            }
        }
    }
}
