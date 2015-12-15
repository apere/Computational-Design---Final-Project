﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using Microsoft.Kinect;

using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;

namespace workshop17
{
    public class MediaWindow
    {
        public int Width = 0;       //width of the viewport in
        public int Height = 0;      //height of the viewport in pixels
        public double MouseX = 0.0; //location of the mouse along X
        public double MouseY = 0.0; //location of the mouse along Y

        BackgroundWorker bw = new BackgroundWorker();


        public KinectHelper kinect = new KinectHelper(true, true, true);
        public List<Person> persons = new List<Person>();
        public List<Memory> memories = new List<Memory>();




        //initialization function. Everything you write here is executed once in the beginning of the program
        public void Initialize()
        {
            kinect.Initialize();
        }

        public void Terminate()
        {
            kinect.Close();
        }

        public double AngleXZ = 300.0;
        public double AngleY = 0.0;
        public double Distance = 4.0;

        public Vector3d Eye;
        public Vector3d Target=new Vector3d(0.0, 0.0, 2.0);
        public Vector3d Up = new Vector3d(0.0, 1.0, 0.0);
        
        //animation function. This contains code executed 20 times per second.
        public void OnFrameUpdate()
        {

            GL.ClearColor(1f, 1f, 1f, 1.0f);
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


            GL.Enable(EnableCap.Lighting); //enable lighting calculations
            GL.Enable(EnableCap.Light0);    //enable the first light
            GL.Light(LightName.Light0, LightParameter.Position, new OpenTK.Vector4((float)Target.X, (float)Target.Y, (float)Target.Z + 20, 2.0f)); //set light position and color
            GL.Light(LightName.Light0, LightParameter.Diffuse, new OpenTK.Vector4(1.0f, 1.0f, 1.0f, 125.0f));
            GL.Light(LightName.Light0, LightParameter.Specular, new OpenTK.Vector4(1.0f, 1.0f, 1.0f, 0.0f));

            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.Diffuse); //enable material
            GL.Enable(EnableCap.ColorMaterial);

            GL.LightModel(LightModelParameter.LightModelTwoSide, 1); //set lighting model 
            GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);


            if (kinect.HasSkeletonData && kinect.HasDepthData)
            {
                //GL.Enable(EnableCap.DepthTest);
                bool identified = false;
                foreach(Body skeleton in kinect.Bodies)  // do I have a person object for each skeleton
                {
                    foreach (Person p in persons)
                    {
                        if(!identified && p.compare(skeleton)) // ignore previously identified person
                        {
                            identified = true; 
                        }
                    }
                    if (!identified && skeleton.TrackingId != 0) // if we haven't identified the person, add them to our persons list.
                    {
                        persons.Add(new Person(skeleton, kinect));
                    }

                    identified = false; // reset 'identified' for next skeleton
                }
            } // END if skeleton data

            bool removed = true;
            List<Person> toRemove = new List<Person>();
            foreach (Person p in persons) // check to see if we are keeping track of people that are no longer in front of the installation and should be turned into memories
            {
                foreach (Body skeleton in kinect.Bodies)
                {
                    if (removed && p.compare(skeleton)) // assume removed until we've found the person's skeleton
                    {
                        removed = false;
                    }
                }

                if (removed) // if the person is no longer physically in front of the installation
                {
                    memories.Add(p.getMemory()); // add person's memory to our list of memories
                    toRemove.Add(p); // remove person from our list of currently tracked persons
                  //  Console.Write("person removed");
                }
                else // if the person is still physically in front of installation and being tracked
                {
                    p.takeSnapshot(); // add to that person's memory
                   // p.getMemory().render(persons);
                }
            }

            foreach(Person p in toRemove)
            {
                persons.Remove(p);
                Console.WriteLine("removed " + p.getID());
            }
            toRemove.Clear();

            foreach (Memory mem in memories) // display each memory
            {
                mem.render(persons); // call the render method for each memory so they can draw themselves to the screen
            }

            Console.WriteLine(" ");
            Console.WriteLine(persons.Count + " persons");
            Console.WriteLine(memories.Count + " memories");
            Console.WriteLine("----");

        } // END onFrameUpdate()
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
