﻿using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;




namespace workshop17
{
    public class Person
    {
        protected Memory myMemory;
        protected ulong id;
        protected Body myBody; // reference to the body object that created this person
        KinectHelper kinect;

        // constructor
        public Person(Body skeleton, KinectHelper k)
        {
            // initialize instance variables & other setup
            myBody = skeleton;
            kinect = k;
            id = myBody.TrackingId;
            myMemory = new Memory(id);
            Console.WriteLine("new person " + id);
        }

        // Function takeSnapshot
        //  This function takes a snapshot of the person as is 
        //      (still unsure whether this is an picture, colored points, whatever) 
        //      and adds that snapshot to the person’s memory.
        public void takeSnapshot()
        {
            List<KinectPoint> points = new List<KinectPoint>();
            if (myBody.Joints != null && myBody.Joints.Count > 0)
            {
                CameraSpacePoint joint = myBody.Joints[0].Position;
                Vector3d jo = new Vector3d(-joint.X, -joint.Y, -joint.Z);

                // checks to see if the point is within range of joint

                for (int j = 0; j < kinect.DepthHeight; j += 1)
                {
                    for (int i = 0; i < kinect.DepthWidth; i += 1)
                    {
                        KinectPoint kp = kinect.Points[i, j];
                        double dist = Math.Abs(Vector3d.Subtract(kp.p, jo).Length);
                        if (dist < 3)
                        {
                            points.Add(kp);
                        }
                    }
                }
                    myMemory.add(points);
                
                Console.Write("ID " + id + " new frame: " + myMemory.Count());
            } else
            {

                Console.Write("no joints");
            }
        }

        // Function getMemory
        //
        // Returns
        //  this person's memory object
        public Memory getMemory()
        {
            return myMemory;
        }

        public ulong getID()
        {
            return id;
        }


        // Function compare
        //  This function takes in a Body object & returns true if it is the body
        //      that corresponds to this person and false otherwise
        //
        // Parameters
        // Body Skeleton - the Body object we are comparing with
        //
        // Return
        //  whether or not the skeleton belongs to this person
        public bool compare(Body Skeleton)
        {
            if (Skeleton != null)
            {
                return this.id == Skeleton.TrackingId;
            }
            return false;

        }
        public Vector3d getPosition() {
            Vector3d ps=new Vector3d(myBody.Joints[0].Position.X, myBody.Joints[0].Position.Y, myBody.Joints[0].Position.Z);
            return ps;
        }
    }
}