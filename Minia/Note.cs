using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Minia {
    public enum NoteType : int {
        Bar,
        Arrow,
    }
    static class Note {
        const NoteType noteType = NoteType.Bar;
        const float barHeight = 0.0f;           //placeholder for settings
        const float sliderWidth = 0.0f;

        public static void DrawNote(float x, float y, float width, Color color) {
            switch (noteType) {
                case NoteType.Bar:
                    Bar(x, y, width, barHeight, color);
                    break;
                case NoteType.Arrow:
                    Arrow(x, y, width, color);
                    break;
                default:
                    break;
            }
        }
        public static void DrawSlider(float x, float y1, float y2, float width, Color color) {
            if (sliderWidth == 0f) {
                Draw.Line(x, y1, x, y2, color);
            }
            else {
                Draw.Rectangle(
                    x - width * sliderWidth / 2,
                    y1, x + width * sliderWidth / 2,
                    y2,
                    color
                );
            }
        }
        private static void Bar(float x, float y, float width, float height, Color color) {
            if (height == 0) {
                Draw.Line(
                    x - width / 2,
                    y,
                    x + width / 2,
                    y,
                    color
                );
            }
            else {
                Draw.Rectangle(
                   x - width / 2,
                   y - height / 2,
                   x + width / 2,
                   y + height / 2,
                   color
               );
            }
        }
        private static void Arrow(float x, float y, float scale, Color color) {
            //float xspace = x2 - x1;
            //float yspace = y2 - y1;
            //Draw.Rectangle(x1 + (xspace / 4), y1, x2 - (xspace / 4), y2 - (yspace / 2), color);
            //GL.Begin(PrimitiveType.Triangles);
            //GL.Color3(color);
            //GL.Vertex2(-x1, y2 - (yspace / 2));
            //GL.Vertex2(-x1 - (xspace / 2), y2);
            //GL.Vertex2(-x2, y2 - (yspace / 2));
            //GL.End();

            //float xspace = x2 - x1;
            //float yspace = y2 - y1;
            //GL.Begin(PrimitiveType.Polygon);
            //GL.Color3(color);
            ////GL.Vertex2(-x1 - (xspace / 4), y1);
            ////GL.Vertex2(-x1 - (xspace / 4), y2 - (yspace / 2));
            //GL.Vertex2(-x1, y2 - (yspace / 2));
            //GL.Vertex2(-x1 - (xspace / 2), y2);
            //GL.Vertex2(-x2, y2 - (yspace / 2));
            //GL.Vertex2(-x2 + (xspace / 4), y2 - (yspace / 2));
            //GL.Vertex2(-x2 + (xspace / 4), y1);
            //GL.Vertex2(-x1 - (xspace / 4), y1);
            //GL.Vertex2(-x1 - (xspace / 4), y2 - (yspace / 2));
            //GL.Vertex2(-x1, y2 - (yspace / 2));


            //GL.End();
            //(x1 | y2)         (x2 | y2)
            //
            //
            //(x1 | y1)         (x2 | y1)

        }
    }
}