using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Minia {
    static class Draw {
        public static void Line(float x1, float y1, float x2, float y2, Color color) {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(color);
            GL.Vertex2(-x1, y1);
            GL.Vertex2(-x2, y2); //still need to figure out why columns are flipped
            GL.End();
        }
        public static void Triangle() {
            //TODO
        }
        public static void Rectangle(float x1, float y1, float x2, float y2, Color color) {
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(color);
            GL.Vertex2(-x2, y1);
            GL.Vertex2(-x1, y1);
            GL.Vertex2(-x1, y2);
            GL.Vertex2(-x2, y2); //still need to figure out why columns are flipped
            GL.End();
        }
    }
}
