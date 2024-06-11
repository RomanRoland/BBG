using System;
using System.Drawing;
namespace BilaLogic
{
    internal class BlackholeBall : Balls
    {
        public BlackholeBall() : this(0, 0, 0, 0, 0, Color.White)
        {
        }
        public BlackholeBall(short radius, short px, short py, short dx, short dy, Color color)
        {
            Color = color;
            Radius = radius;
            Px = px; Py = py;
            Dx = dx; Dy = dy;
        }
        public static BlackholeBall Generate()
        {
            Random Generator = new Random();
            short radius = (short)Generator.Next(75, 175);
            Color color = Color.White;
            BlackholeBall b = new BlackholeBall(radius,
                                            (short)Generator.Next(Form1.Instance.pictureBox1.Height - radius),
                                            (short)Generator.Next(Form1.Instance.pictureBox1.Height - radius),
                                            0,
                                            0,
                                            color);
            return b;
        }
    }
}