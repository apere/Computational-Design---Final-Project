using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using Microsoft.Kinect;

using System.Windows.Forms;

namespace workshop17
{
    public class MediaWindow
    {
        public int Width = 0;       //width of the viewport in
        public int Height = 0;      //height of the viewport in pixels
        public double MouseX = 0.0; //location of the mouse along X
        public double MouseY = 0.0; //location of the mouse along Y


        public KinectHelper kinect = new KinectHelper(true, true, true);

        //initialization function. Everything you write here is executed once in the begining of the program
        public void Initialize()
        {
            kinect.Initialize();
        }

        public void Terminate()
        {
            kinect.Close();
        }

        public double AngleXZ = 0.0;
        public double AngleY = 0.0;
        public double Distance = 4.0;

        public Vector3d Eye;
        public Vector3d Target=new Vector3d(0.0, 0.0, 2.0);
        public Vector3d Up = new Vector3d(0.0, 1.0, 0.0);
        
        //animation function. This contains code executed 20 times per second.
        public void OnFrameUpdate()
        {



            GL.ClearColor(0.6f, 0.6f, 0.6f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            Matrix4d pmat = Matrix4d.Perspective(Math.PI * 0.35, Width / Height, 0.1, 100.0);
            GL.LoadMatrix(ref pmat);

            GL.MatrixMode(MatrixMode.Modelview);
            Eye.X = Target.X + Distance * Math.Cos(AngleXZ) * Math.Cos(AngleY);
            Eye.Y = Target.Y + Distance * Math.Sin(AngleY); 
            Eye.Z = Target.Z + Distance * Math.Sin(AngleXZ) * Math.Cos(AngleY);

            Matrix4d vmat = Matrix4d.LookAt(Eye, Target, Up);
            GL.LoadMatrix(ref vmat);


            

           

            if (kinect.HasDepthData)
            {
                GL.Enable(EnableCap.DepthTest);
                GL.PointSize(2.0f);
                GL.Begin(PrimitiveType.Points);
                for (int j = 0; j < kinect.DepthHeight; j+=5)
                {
                    for (int i = 0; i < kinect.DepthWidth; i+=5)
                    {
                       KinectPoint kp=  kinect.Points[i, j];
            
                        GL.Color4(kp.color);
                        GL.Vertex3(kp.p);
                        
                    }
                }
                GL.End();
            }


            if (kinect.HasSkeletonData)
            {
                


                GL.Enable(EnableCap.DepthTest);
                GL.Begin(PrimitiveType.TriangleFan);
                foreach (Body b in kinect.Bodies)
                {
                    if (!b.IsTracked) continue;
                    foreach (Joint j in b.Joints.Values)
                    {
                       
                        float x = j.Position.X;
                        float y = j.Position.Y;
                        float z = j.Position.Z;

                        GL.Color4(kinect.ColorAt(j.Position));
                        GL.Vertex3(x, y, z);
                    }
                }
                GL.End();



                GL.Disable(EnableCap.DepthTest);
                GL.Color4(1.0, 1.0, 1.0, 1.0);
                GL.PointSize(10.0f);
                GL.Begin(PrimitiveType.Points);
                foreach (Body b in kinect.Bodies)
                {
                    if (!b.IsTracked) continue;
                    foreach (Joint j in b.Joints.Values)
                    {
                        float x = j.Position.X;
                        float y = j.Position.Y;
                        float z = j.Position.Z;

                        GL.Vertex3(x, y, z);
                    }
                }
                GL.End();
            }

        }
        
        double mouseX0 = 0.0;
        double mouseY0 = 0.0;

        
        public void MouseMove(double x, double y, MouseButtons button)
        {
            mouseX0 = MouseX;
            mouseY0 = MouseY;

            MouseX = x;
            MouseY = y;

            double dx = MouseX - mouseX0;
            double dy = MouseY - mouseY0;

            if (button == MouseButtons.Left)
            {
                AngleXZ += dx*0.01;
                AngleY += dy*0.01;
            }   
            else if (button== MouseButtons.Right)
            {
                Distance += dy * 0.01;
            }
        }
    }
}
