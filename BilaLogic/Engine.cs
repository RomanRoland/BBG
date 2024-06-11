using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace BilaLogic
{
    internal static class Engine
    {
        internal static Balls[][] StartSimulation()
        {
            Bitmap bitmap = new Bitmap(Form1.Instance.pictureBox1.Width, Form1.Instance.pictureBox1.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            RegularBall[] regularBalls = new RegularBall[5];
            RepelentBall[] repelentBalls = new RepelentBall[3];
            BlackholeBall[] blackholeBalls = new BlackholeBall[2];
            for (int i = 0;  i < regularBalls.Length; i++)
            {
                regularBalls[i] = RegularBall.Generate();
                Thread.Sleep(1);
                if (Check(regularBalls, regularBalls[i], i))
                {
                    regularBalls[i] = null;
                    i--;
                }
            }
            for (int i = 0; i < repelentBalls.Length; i++)
            {
                repelentBalls[i] = RepelentBall.Generate();
                Thread.Sleep(1);
                if (Check(regularBalls, repelentBalls[i], regularBalls.Length) 
                    || Check(repelentBalls, repelentBalls[i], i))
                {
                    repelentBalls[i] = null;
                    i--;
                }
            }
            for (int i = 0; i < blackholeBalls.Length; i++)
            {
                blackholeBalls[i] = BlackholeBall.Generate();
                Thread.Sleep(1);
                if (Check(regularBalls, blackholeBalls[i], regularBalls.Length)
                    || Check(repelentBalls, blackholeBalls[i], repelentBalls.Length)
                    || Check(blackholeBalls,blackholeBalls[i],i))
                {
                    blackholeBalls[i] = null;
                    i--;
                }
            }
            CircleDraw(graphics, regularBalls);
            CircleDraw(graphics, repelentBalls);
            CircleDraw(graphics, blackholeBalls);
            Form1.Instance.pictureBox1.Image = bitmap;
            Thread.Sleep(1);
            Form1.Instance.Update();
            Balls[][] balls = new Balls[][]
            {
                regularBalls,
                repelentBalls,
                blackholeBalls
            };
            return balls;
        }

        public static bool Simulation(ref Balls[][] balls, int height)
        {
            Bitmap bitmap = new Bitmap(height, height);
            Graphics graphics = Graphics.FromImage(bitmap);
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < balls[j].Length; i++)
                {
                    balls[j][i].Px += balls[j][i].Dx;
                    balls[j][i].Py += balls[j][i].Dy;
                    if (balls[j][i].Px <= 0)
                    {
                        balls[j][i].Px *= -1;
                        balls[j][i].Dx *= -1;
                    }
                    if (balls[j][i].Px >= height - 27 - balls[j][i].Radius)
                    {
                        balls[j][i].Px -= (short)(balls[j][i].Px - height + balls[j][i].Radius * 2);
                        balls[j][i].Dx *= -1; 
                    }
                    if (balls[j][i].Py <= 0)
                    {
                        balls[j][i].Py *= -1;
                        balls[j][i].Dy *= -1;
                    }
                    if (balls[j][i].Py >= height - 120 - balls[j][i].Radius)
                    {
                        //MessageBox.Show("test");
                        balls[j][i].Py -= (short)(balls[j][i].Py - height + balls[j][i].Radius * 2);
                        balls[j][i].Dy *= -1;
                    }
                }
            }
            for (int i = 0; i < balls[0].Length - 1; i++)
            {
                Balls check = InteractionCheck(balls[0], balls[0][i], balls[0].Length, i+1);
                if (check != null)
                {
                    if (balls[0][i].Radius > check.Radius)
                    {
                        balls[0][i] = RegularVsRegular(balls[0][i], check);
                        balls[0] = DiscardElement(check, balls[0]);
                    }
                    else
                    {
                        balls[0][i] = RegularVsRegular(check, balls[0][i]);
                        balls[0] = DiscardElement(balls[0][i], balls[0]);
                    }
                    continue;
                }
                if (balls[0].Length == 0)
                    return false;
                check = InteractionCheck(balls[1], balls[0][i], balls[1].Length, 0);
                if (check != null)
                {
                    (check.Color,balls[0][i].Color) = (balls[0][i].Color, check.Color);
                    balls[0][i].Dx *= -1;
                    balls[0][i].Dy *= -1;
                    check.Dx *= -1;
                    check.Dy *= -1;
                }
                check = InteractionCheck(balls[2], balls[0][i], balls[2].Length, 0);
                if (check != null)
                {
                    check.Radius += balls[0][i].Radius;
                    balls[0] = DiscardElement(balls[0][i], balls[0]);
                }
                if (balls[0].Length == 0)
                    return false;
            }
            for (int i = 0; i < balls[1].Length - 1; i++)
            {
                Balls check = InteractionCheck(balls[1], balls[1][i], balls[1].Length, i + 1);
                if (check != null)
                {
                    (check.Color, balls[1][i].Color) = (balls[1][i].Color, check.Color);
                }
                check = InteractionCheck(balls[2], balls[1][i], balls[2].Length, 0);
                if (check != null)
                {
                    balls[1][i].Radius /= 2;
                }
            }
            CircleDraw(graphics, balls[0]);
            CircleDraw(graphics, balls[1]);
            CircleDraw(graphics, balls[2]);
            Form1.Instance.pictureBox1.Image = bitmap;
            Thread.Sleep(500);
            Form1.Instance.Update();
            if (balls[0].Length == 0)
                return false;
            return true;
        }

        private static Balls[] DiscardElement(Balls check, Balls[] balls)
        {
            sbyte index = 0;
            Balls[] result = new Balls[balls.Length - 1];
            for (sbyte i = 0; i < balls.Length; i++)
            {
                if (check.Px == balls[i].Px && check.Py == balls[i].Py)
                {
                    index = i;
                    break;
                }
                result[i] = balls[i];
            }
            for (sbyte i = index; i < balls.Length - 1; i++)
            {
                result[i] = balls[i + 1];
            }
            return result;
        }

        private static Balls RegularVsRegular(Balls balls, Balls check)
        {
            balls.Radius += check.Radius;
            balls.Color = Color.FromArgb((balls.Color.R + check.Color.R) / 2, 
                                         (balls.Color.G + check.Color.G) / 2, 
                                         (balls.Color.B + check.Color.B) / 2);
            return balls;
        }

        private static Graphics CircleDraw(Graphics graphics, Balls[] balls)
        {
            for (int i = 0; i < balls.Length; i++) 
            {
                Brush brush = new SolidBrush(balls[i].Color);
                graphics.FillEllipse(brush, balls[i].Px, balls[i].Py, balls[i].Radius, balls[i].Radius);
            }
            return graphics;
        }

        private static bool Check(Balls[] Balls, Balls checkedBall,int lenght)
        {
            for (int i = 0; i < lenght; i++)
            {
                if ((double)(Balls[i].Radius + checkedBall.Radius) >= Distance(Balls[i], checkedBall))
                {
                     return true;
                }
            }
            return false;
        }
        private static Balls InteractionCheck(Balls[] Balls, Balls checkedBall, int lenght, int start)
        {
            for (int i = start; i < lenght; i++)
            {
                if ((double)(Balls[i].Radius + checkedBall.Radius)/2 >= Distance(Balls[i], checkedBall))
                {
                    return Balls[i];
                }
            }
            return null;
        }

        private static double Distance(Balls Balli, Balls Ballj)
        {
            return Math.Sqrt(Math.Pow(Balli.Px - Ballj.Px, 2) + Math.Pow(Balli.Py - Ballj.Py, 2));
        }
    }
}
