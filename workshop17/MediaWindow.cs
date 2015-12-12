﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using Microsoft.Kinect;

//using Rhino.Geometry;

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
        public List<Person> persons = new List<Person>();
        public List<Memory> memories = new List<Memory>();
        public Background b;


        //initialization function. Everything you write here is executed once in the beginning of the program
        public void Initialize()
        {
            b = new Background();
            kinect.Initialize();
        }

        public void Terminate()
        {
            kinect.Close();
        }

        public double AngleXZ = 0.0;
        public double AngleY = 0.0;
        public double Distance = 30.0;

        public OpenTK.Vector3d Eye;
        public OpenTK.Vector3d Target =new OpenTK.Vector3d(5.0, 0.0,0.0);
        public OpenTK.Vector3d Up = new OpenTK.Vector3d(0.0, 2.0, 0.0);
        
        //animation function. This contains code executed 20 times per second.
        public void OnFrameUpdate()
        {

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
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

            //................................................................................Set Up Lighting

            GL.Enable(EnableCap.Lighting);      //enable lighting calculations
            GL.Enable(EnableCap.Light0);        //enable the first light
            GL.Light(LightName.Light0, LightParameter.Position, new OpenTK.Vector4(30.0f, 30.0f, 30.0f, 1.0f)); //set light position and color
            GL.Light(LightName.Light0, LightParameter.Diffuse, new OpenTK.Vector4(1.0f, 1.0f, 1.0f, 1.0f));

            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.Diffuse); //enable material
            GL.Enable(EnableCap.ColorMaterial);

            GL.LightModel(LightModelParameter.LightModelTwoSide, 1); //set lighting model 
            GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);

            b.OnframeUpdate();

            if (kinect.HasDepthData)
            {
                GL.Enable(EnableCap.DepthTest);
                GL.PointSize(2.0f);
                GL.Begin(PrimitiveType.Points);
                // do stuff w/ depth data
                GL.End();
            }


            if (kinect.HasSkeletonData)
            {
                // CODE GOES HERE
              
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

                AngleXZ += dx * 0.01;

                AngleY += dy * 0.01;

            }

            else if (button == MouseButtons.Right)

            {

                Distance += dy * 0.01;

            }
        }
    }
}
