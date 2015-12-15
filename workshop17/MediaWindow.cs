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

/// <summary>
/// Media Window
/// Authors: Adam M Pere, Keebaik Sim, Enol Vallina
/// Introduction to Computational Design - Fall 2015, Harvard Graduate School of Design
/// 
/// The MediaWindow class is where the OpenGL scene is set up and where the main communication with the Kinect Sensor happens.
/// </summary>

namespace workshop17
{
    public class MediaWindow
    {
        public int Width = 0;       //width of the viewport in pixels
        public int Height = 0;      //height of the viewport in pixels
        public double MouseX = 0.0; //location of the mouse along X
        public double MouseY = 0.0; //location of the mouse along Y

        public KinectHelper kinect = new KinectHelper(true, true, true);  // wrapper object for kinect sensor
        public List<Person> persons = new List<Person>();                 // list of people currently standing in front of kinect sensor
        public List<Memory> memories = new List<Memory>();                // list of the 'memories' of people who once stood in front of the kinect sensor

        public List<List<Vector3d>> allJoints = new List<List<Vector3d>>();         // list of all joints currently being detected - separated by user
        public List<List<KinectPoint>> allPoints = new List<List<KinectPoint>>();   // list of all points within a certain distance of a joint - separated by user

        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize()
        {
            kinect.Initialize();
        }

        /// <summary>
        /// Terminate
        /// </summary>
        public void Terminate()
        {
            kinect.Close();
        }

        // Camera Variables
        public double AngleXZ = 300.0;
        public double AngleY = 0.0;
        public double Distance = 4.0;

        public Vector3d Eye;
        public Vector3d Target=new Vector3d(0.0, 0.0, 2.0);
        public Vector3d Up = new Vector3d(0.0, 1.0, 0.0);

        double mouseX0 = 0.0;
        double mouseY0 = 0.0;


        /// <summary>
        /// onFrameUpdate
        /// This is the animation function. It is called ~20x per second to update the screen
        /// </summary>
        public void OnFrameUpdate()
        {
            // Scene Setup
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


            // Any new people standing in front of the kinect?
            if (kinect.HasSkeletonData && kinect.HasDepthData)
            {
                addSkeletons();
                allPoints = findPoints();
            }

            // Remove or take a snapshot of any person currently in front of the kinect
            removeOrSnap();

            // Render each memory
            foreach (Memory mem in memories)
            {
                mem.render();
            }

            Console.WriteLine(" ");
            Console.WriteLine(persons.Count + " persons");
            Console.WriteLine(memories.Count + " memories");
            Console.WriteLine("----");
            allJoints.Clear();
            //allPoints.Clear();

        } 


        /// <summary>
        /// MouseMove
        /// Use the mouse to change the camera's view
        /// </summary>
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

        /// <summary>
        /// findPoints
        /// </summary>
        /// <return>a list of kinect points associated with all joint data stored in the variable allJoints</return> 
        public List<List<KinectPoint>> findPoints()
        {
            List<List<KinectPoint>> allKPoints = new List<List<KinectPoint>>();
            List<KinectPoint> tempPoints;
            KinectPoint kp;
            bool add;
            double dist;
            int step = 3;

            if (allJoints != null && allJoints.Count > 0) // error checking
            {
                foreach (List<Vector3d> jointPoints in allJoints) // loop through set of joints (each set is one user's joints)
                {
                    tempPoints = new List<KinectPoint>();
                    allKPoints.Add(tempPoints);

                    if (jointPoints != null && jointPoints.Count > 0) // error checking
                    {
                        for (int j = 0; j < kinect.DepthHeight; j += step) // loop through every kinect point, saving the ones within a certain distance
                        {
                            for (int k = 0; k < kinect.DepthWidth; k += step)
                            {
                                kp = kinect.Points[k, j];
                                add = false;
                                foreach (Vector3d jointPoint in jointPoints) // loop through every joint of this user comparing distance to point
                                {
                                    if (!add)
                                    {
                                        dist = Math.Abs(Vector3d.Subtract(kp.p, jointPoint).Length);
                                        if (dist <= .25) // if the point is within the threshold, add it
                                        {
                                            add = true;
                                        }
                                    }
                                }

                                if (add)
                                {
                                    tempPoints.Add(kp);
                                }
                            }
                        }
                    }
                }
            }
            
            return allKPoints;
        }

        /// <summary>
        /// addSkeletons
        /// Gets the joint data for every person object ---- may have to do this after add (or remove).
        /// If it finds a person who has just been detected by the kinect this frame, it will add them to our list of persons
        /// </summary>
        public void addSkeletons()
        {
            bool identified = false;
            bool getJoints = true;
            foreach (Body skeleton in kinect.Bodies)  // do I have a person object for each skeleton
            {
                foreach (Person p in persons)
                {
                    if (getJoints) // collect joint data for every person
                    {
                        getJoints = false;
                        allJoints.Add(p.getJoints());
                    }
                    if (!identified && p.compare(skeleton)) // ignore previously identified person
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
        }

        /// <summary>
        /// removeOrSnap
        /// This function either removes the person object (and stores their memory) of a person no longer being tracked by the kinect 
        /// OR it invokes taking a snapshot of that person
        /// </summary>
        public void removeOrSnap()
        {
            bool removed = true;
            List<Person> toRemove = new List<Person>();
            Person pp;
            for (int i = 0; i < persons.Count; i++) // check to see if we are keeping track of people that are no longer in front of the installation and should be removed (turned into memories)
            {
                pp = persons[i];
                foreach (Body skeleton in kinect.Bodies)
                {
                    if (removed && pp.compare(skeleton)) // assume removed until we've found the person's skeleton
                    {
                        removed = false;
                    }
                }

                if (removed) // if the person is no longer physically in front of the installation
                {
                    memories.Add(pp.getMemory()); // add person's memory to our list of memories
                    toRemove.Add(pp); // remove person from our list of currently tracked persons
                                      //  Console.Write("person removed");
                }
                else // if the person is still physically in front of installation and being tracked
                {                    
                    if(i < allPoints.Count && allPoints[i] != null && allPoints[i].Count > 0)
                    {
                        pp.takeSnapshot(allPoints[i]); // add to that person's memory
                    }
                   
                }
            }

            foreach (Person p in toRemove)
            {
                persons.Remove(p);
                Console.WriteLine("removed " + p.getID());
            }
            toRemove.Clear();
        }
    }
}