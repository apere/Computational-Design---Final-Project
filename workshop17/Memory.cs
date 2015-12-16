using System;
using Microsoft.Kinect;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

<<<<<<< HEAD
=======

>>>>>>> parent of 665b8ce... updated comments etc
namespace workshop17
{
    /// <summary>
    /// Memory Class
    /// Authors: Adam M Pere, Keebaik Sim, Enol Vallina
    /// Introduction to Computational Design - Fall 2015, Harvard Graduate School of Design
    /// 
    /// The memory class represents a recording of people that were once standing in front of the kinect sensor but are no longer.
    /// The 'memory' of a person object.
    /// </summary>
    public class Memory
    {
        DateTime timeCreated;
        Vector3d Location;
        ulong id;
        //List<images> frames; // each index represents a collection of snapshots or photos taken of the person
        int currentFrame; // the index of the snapshot that should be displayed next time render is called
        List<List<KinectPoint>> snapshots;

        // Constructor
        public Memory(ulong i) // parameters?
        {
            timeCreated = DateTime.Now;
            // initialize instance variables & other setup
            snapshots = new List<List<KinectPoint>>();
            currentFrame = 0;
            id = i;
        }

        // Function add
        //  This function is called to add a ‘snap shot’ to our array of frames. This should only be called from a person object.
        // Parameters
        //  image - an image that we want to add to this memory
        public void add(List<KinectPoint> snapshotPoints) // parameters?
        {
            snapshots.Add(snapshotPoints);
        }


        // Function getClosestPerson
        //  This function loops through the supplied array of Person objects and 
        //      returns the one physically closest to this memory
        // Parameters
        //  persons - a list of person objects
        // Return
        //  the person in persons that is physically closest to this memory
        public Person getClosestPerson(List<Person> persons)
        {  
            Person closestPerson=persons[0];
            Vector3d position = closestPerson.getPosition();
            float dist = (float)Math.Sqrt(Math.Pow(position.X + Location.X, 2) + Math.Pow(position.Y + Location.Y, 2) + Math.Pow(position.Z + Location.Z, 2)); ;
            float closestDist = dist;

            foreach (Person p in persons)
            {
                position = p.getPosition();
                dist = (float)Math.Sqrt(Math.Pow(position.X + Location.X,2) + Math.Pow(position.Y + Location.Y,2) + Math.Pow(position.Z + Location.Z,2));
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestPerson = p;
                }
            }
            // must return a person
            return closestPerson;
        }

        public int Count()
        {
            return snapshots.Count;
        }

        // Function getOrientation
        //
        // Return
        //  A Vector pointing in the direction that we want to face
        public Vector3d getOrientation(List<Person> persons)
        {
            return this.getClosestPerson(persons).getPosition().Normalized();
        }

        // Function render
        // This function should be called every time we want to display and update the memory. 
        // This should calculate the correct orientation and display the current frame.
        public void render(List<Person> persons)
        {
            if(currentFrame >= snapshots.Count)
            {
                Console.WriteLine("too much " + currentFrame);
            }
            List < KinectPoint > currShot = snapshots[currentFrame];
            double zIndex = currShot[0].p.Z;
            double offset = (DateTime.Now - timeCreated).TotalSeconds / 20; // time offset for z-axis
            double buzz = (DateTime.Now - timeCreated).Seconds/20;

            GL.PointSize(4);  // Changing point size gives some cool abstract results
            GL.Enable(EnableCap.DepthTest);
            GL.Begin(PrimitiveType.Points);
            // PrimitiveType.LineStrip  --- sorta nice
            // PrimitiveType.QuadStrip -- even cooler
            // PrimitiveType.TriangleFan -- weird but preserves face
            // PrimitiveType.Triangles -- weird noisy silhouette
            // 
            Random rand = new Random();
            
            foreach (KinectPoint kp in currShot)
            {
                GL.Color4(kp.color);
                GL.Vertex3(kp.p.X,
                    kp.p.Y,
                    kp.p.Z + offset);
            }
            GL.End();
           
<<<<<<< HEAD
            //Console.WriteLine("mem " + id + " frame:" + currentFrame); // debugging
        
            currentFrame = currentFrame + 1;
            if(currentFrame >= snapshots.Count) // start over?
=======
            //Console.WriteLine("mem" + id + " frame:" + currentFrame);
        
            currentFrame = currentFrame + 1;
            if(currentFrame >= snapshots.Count)
>>>>>>> parent of 665b8ce... updated comments etc
            {
                currentFrame = 0;
            } 
        }

    }
}