using System;
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

        // Function getJoints
        //
        // Returns a list of joints & their location in 3-space
        public List<Vector3d> getJoints()
        {
            CameraSpacePoint joint;
            IEnumerable<JointType> keys = myBody.Joints.Keys;
            List<Vector3d> jointPoints = new List<Vector3d>();
            foreach (JointType key in keys)
            {
                joint = myBody.Joints[key].Position;
                jointPoints.Add(new Vector3d(joint.X, joint.Y, joint.Z));

            }

            return jointPoints;
        }

        // Function takeSnapshot
        //  This function takes a snapshot of the person as is 
        //      (still unsure whether this is an picture, colored points, whatever) 
        //      and adds that snapshot to the person’s memory.
        public void takeSnapshot()
        {
            List<KinectPoint> points = new List<KinectPoint>();
            double dist;
            KinectPoint kp;
            bool add;
            List<Vector3d> jointPoints = getJoints();
            if (myBody.Joints != null && myBody.Joints.Count > 0)
            {
                // checks to see if the point is within range of joint

                for (int j = 0; j < kinect.DepthHeight; j += 3)
                {
                    for (int i = 0; i < kinect.DepthWidth; i += 3)
                    {
                        kp = kinect.Points[i, j];
                        add = false;
                        foreach(Vector3d jointPoint in jointPoints)
                        {
                            if (!add)
                            {
                                dist = Math.Abs(Vector3d.Subtract(kp.p, jointPoint).Length);
                                if (dist <= .3)
                                { 
                                    add = true;
                                }
                            }
                        }
                        
                        if (add)
                        {
                            points.Add(kp);
                        }
                    }
                }
                    myMemory.add(points);
  
                
                Console.WriteLine("ID " + id + " new frame: " + myMemory.Count() + " " + " points");
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