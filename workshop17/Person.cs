using System;
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

        // constructor
        public Person(Body skeleton)
        {
            // initialize instance variables & other setup
        }

        // Function takeSnapshot
        //  This function takes a snapshot of the person as is 
        //      (still unsure whether this is an picture, colored points, whatever) 
        //      and adds that snapshot to the person’s memory.
        public void takeSnapshot()
        {

        }

        // Function getMemory
        //
        // Returns
        //  this person's memory object
        public Memory getMemory()
        {
            return myMemory;
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