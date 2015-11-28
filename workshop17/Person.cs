using System;
using Microsoft.Kinect;

namespace workshop17
{
    public class Person
    {
        protected Memory myMemory;
        protected int id;
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
        public bool compare(Body Skelton)
        {
            return true;
        }
    }
}