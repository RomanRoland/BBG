using System;
using System.Drawing;
namespace BilaLogic
{
    internal class RepelentBall : Balls
    {
        public RepelentBall() : this(0, 0, 0,0, 0, Color.White)
        {
        }
        public RepelentBall(short radius, short px, short py, short dx, short dy, Color color)
        {
            Color = color;
            Radius = radius;
            Px = px; Py = py;
            Dx = dx; Dy = dy;
        }
        public static RepelentBall Generate()
        {
            Random Generator = new Random();
            short radius = (short)Generator.Next(80, 100);
            //Color color = Color.FromArgb(Generator.Next(255), Generator.Next(255), Generator.Next(255));
            RepelentBall b = new RepelentBall(radius,
                                            (short)Generator.Next(Form1.Instance.pictureBox1.Height),
                                            (short)Generator.Next(Form1.Instance.pictureBox1.Height),
                                            (short)Generator.Next(-20, 20),
                                            (short)Generator.Next(-20, 20),
                                            /*color*/Color.Yellow);
            return b;
        }
    }
}