using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Minia {
    static class Shapes {
        public static void Line(float x1, float y1, float x2, float y2, Color color) {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(color);
            GL.Vertex2(x1, y1);
            GL.Vertex2(x2, y2);
            GL.End();
        }

        public static void Triangle() {
            //TODO
        }

        public static void Rectangle(float x1, float y1, float x2, float y2, Color color) {
            GL.Begin(PrimitiveType.Quads);
            GL.Color3((byte)(color.R / 2), (byte)(color.G / (byte)2), (byte)(color.B / (byte)2));
            //GL.Color3(255, 255, 255);
            GL.Vertex2(x2, y1);
            GL.Vertex2(x1, y1);
            GL.Vertex2(x1, y2);
            GL.Vertex2(x2, y2);
            GL.End();
        }

        public static void Text(string text, float x, float y, float scale, Color color) {
            for (int i = 0; i < text.Length; i++) {
                var character = text[i];
                Character(character, x + scale*0.5f*i, y - scale / 2f, scale, color);
            }
        }

        private static void Character(char character, float x, float y, float scale, Color color) {
            var width = scale * 0.33f;
            var height = scale * 0.8f;
            switch (character) {
                case 'A':
                case 'a':
                    Line(x, y, x + width / 2f, y + height, color);
                    Line(x + width, y, x + width / 2f, y + height, color);
                    Line(x + width / 4f, y + height / 2f, x + width * 3f / 4f, y + height / 2f, color);
                    break;
                case 'B':
                case 'b':
                    Line(x, y, x, y + height, color);
                    Line(x, y, x + width, y + height / 4f, color);
                    Line(x + width, y + height / 4f, x, y + height / 2f, color);
                    Line(x, y + height / 2f, x + width, y + height * 3f / 4f, color);
                    Line(x + width, y + height * 3f / 4f, x, y+height, color);
                    break;
                case 'C':
                case 'c':
                    Line(x + width, y + height, x + width / 2f, y + height, color);
                    Line(x + width / 2f, y + height, x, y + height / 2f, color);
                    Line(x, y + height / 2f, x + width / 2f, y, color);
                    Line(x + width / 2f, y, x + width, y, color);
                    break;
                case 'D':
                case 'd':
                    Line(x, y, x, y + height, color);
                    Line(x, y, x + width / 2f, y, color);
                    Line(x + width / 2f, y, x + width, y + height / 2f, color);
                    Line(x + width, y + height / 2f, x + width * 0.5f, y + height, color);
                    Line(x, y + height, x + width / 2f, y + height, color);
                    break;
                case 'E':
                case 'e':
                    Character('F', x, y, scale, color);
                    Line(x, y, x + width, y, color);
                    break;
                case 'F':
                case 'f':
                    Line(x, y, x, y + height, color);
                    Line(x, y + height, x + width, y + height, color);
                    Line(x, y + height / 2f, x + width / 2f, y + height / 2f, color);
                    break;
                case 'G':
                case 'g':
                    Character('C', x, y, scale, color);
                    Line(x + width, y, x + width, y + height / 2f, color);
                    Line(x + width, y + height / 2f, x+width/2f, y+height/2f, color);
                    break;
                case 'H':
                case 'h':
                    Line(x, y, x, y + height, color);
                    Line(x + width, y, x + width, y + height, color);
                    Line(x, y + height / 2f, x + width, y + height / 2f, color);
                    break;
                case 'I':
                case 'i':
                    Line(x + width / 2f, y, x + width / 2f, y + height, color);
                    break;
                case 'J':
                case 'j':
                    Line(x, y + height, x + width, y + height, color);
                    Line(x + width, y + height, x+width, y + height/2f, color);
                    Line(x + width, y + height / 2f, x+width/2f, y, color);
                    Line(x + width / 2f, y, x, y+height/2f, color);
                    break;
                case 'K':
                case 'k':
                    Line(x, y, x, y + height, color);
                    Line(x, y + height / 2f, x + width, y + height, color);
                    Line(x, y + height / 2f, x + width, y, color);
                    break;
                case 'L':
                case 'l':
                    Line(x, y, x, y + height, color);
                    Line(x, y, x + width, y, color);
                    break;
                case 'M':
                case 'm':
                    Line(x, y, x, y + height, color);
                    Line(x, y + height, x + width / 2f, y + height / 2f, color);
                    Line(x + width / 2f, y + height / 2f, x+width, y+height, color);
                    Line(x + width, y, x + width, y + height, color);
                    break;
                case 'N':
                case 'n':
                    Line(x, y, x, y + height, color);
                    Line(x, y + height, x + width, y, color);
                    Line(x + width, y, x + width, y + height, color);
                    break;
                case 'O':
                case 'o':
                    Line(x, y + height / 2f, x + width / 2f, y + height, color);
                    Line(x + width / 2f, y + height, x + width, y + height / 2f, color);
                    Line(x + width, y + height / 2f, x + width / 2f, y, color);
                    Line(x + width / 2f, y, x, y + height / 2f, color);
                    break;
                case 'P':
                case 'p':
                    Line(x, y, x, y + height, color);
                    Line(x, y + height, x + width, y + height * 0.75f, color);
                    Line(x, y + height/2f, x + width, y + height * 0.75f, color);
                    break;
                case 'Q':
                case 'q':
                    Character('O', x, y, scale, color);
                    Line(x + width / 2f, y + height / 2f, +width, y, color);
                    break;
                case 'R':
                case 'r':
                    Character('P', x, y, scale, color);
                    Line(x, y + height / 2f, x + width, y, color);
                    break;
                case 'S':
                case 's':
                    Line(x, y+height/4f, x + width / 2f, y, color);
                    Line(x + width / 2f, y, x + width, y + height / 4f, color);
                    Line(x + width, y + height / 4f, x, y + height*0.75f, color);
                    Line(x, y + height * 0.75f, x + width / 2f, y + height, color);
                    Line(x + width / 2f, y + height, x+width, y+height*0.75f, color);
                    break;
                case 'T':
                case 't':
                    Character('I', x, y, scale, color);
                    Line(x, y + height, x + width, y + height, color);
                    break;
                case 'U':
                case 'u':
                    Line(x, y + height, x, y + height / 2f, color);
                    Line(x, y + height / 2f, x + width / 2f, y, color);
                    Line(x + width, y + height / 2f, x + width / 2f, y, color);
                    Line(x + width, y + height, x + width, y + height / 2f, color);
                    break;
                case 'V':
                case 'v':
                    Line(x, y + height, x + width / 2f, y, color);
                    Line(x + width, y + height, x + width / 2f, y, color);
                    break;
                case 'W':
                case 'w':
                    Line(x, y, x, y + height, color);
                    Line(x, y, x + width / 2f, y + height / 2f, color);
                    Line(x + width / 2f, y + height / 2f, x + width, y, color);
                    Line(x + width, y, x + width, y + height, color);
                    break;
                case 'X':
                case 'x':
                    Line(x, y, x + width, y + height, color);
                    Line(x + width, y, x, y + height, color);
                    break;
                case 'Y':
                case 'y':
                    Line(x, y + height, x + width / 2f, y + height / 2f, color);
                    Line(x+width, y + height, x + width / 2f, y + height / 2f, color);
                    Line(x + width / 2f, y, x + width / 2f, y + height / 2f, color);
                    break;
                case 'Z':
                case 'z':
                    Line(x, y + height, x + width, y + height, color);
                    Line(x + width, y + height, x, y, color);
                    Line(x, y, x + width, y, color);
                    break;
                case '0':
                    Character('O', x, y, scale, color);
                    break;
                case '1':
                    Line(x + width / 2f, y, x + width / 2f, y + height, color);
                    break;
                case '2':
                    break;
                case '3':
                    break;
                case '4':
                    break;
                case '5':
                    break;
                case '6':
                    break;
                case '7':
                    break;
                case '8':
                    break;
                case '9':
                    break;
                case '-':
                    Line(x, y + height / 2f, x + width, y + height / 2f, color);
                    break;
                case '_':
                    Line(x, y, x + width, y, color);
                    break;
                default:
                    break;
            }
        }
    }
}
