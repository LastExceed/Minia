using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Minia {
    static class Note {
        public enum Type : int {
            Rectangle,
            Arrow,
        }

        public static void DrawNote(int type, float x1, float y1, float x2, float y2, Color color) {
            if (type == (int)Type.Rectangle) {
                Rectangle(x1, y1, x2, y2, color);
            }
            else {
                Arrow(x1, y1, x2, y2, color);
            }
        }

        private static void Rectangle(float x1, float y1, float x2, float y2, Color color) {
            if (y1 == y2) {
                GL.Begin(PrimitiveType.Lines);
                GL.Color3(color);
                GL.Vertex2(-x1, y1);
            }
            else {
                GL.Begin(PrimitiveType.Quads);
                GL.Color3(color);
                GL.Vertex2(-x2, y1);
                GL.Vertex2(-x1, y1);
                GL.Vertex2(-x1, y2);
            }
            GL.Vertex2(-x2, y2); //still need to figure out why columns are flipped
            GL.End();
        }

        private static void Arrow(float x1, float y1, float x2, float y2, Color color) {
            float xspace = x2 - x1;
            float yspace = y2 - y1;
            Rectangle(x1 + (xspace / 4), y1, x2 - (xspace / 4), y2 - (yspace / 2), color);
            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(color);
            GL.Vertex2(-x1, y2 - (yspace / 2));
            GL.Vertex2(-x1 - (xspace / 2), y2);
            GL.Vertex2(-x2, y2 - (yspace / 2));
            GL.End();

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