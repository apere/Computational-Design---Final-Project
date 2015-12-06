using System;
using Microsoft.Kinect;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace workshop17
{
    public class Memory
    {
        DateTime timeCreated;
        Vector3d Location;
        ulong id;
        //List<images> frames; // each index represents a collection of snapshots or photos taken of the person
        int currentFrame; // the index of the snapshot that should be displayed next time render is called
        List<List<KinectPoint>> snapshots;
       
        // Constructor
        public Memory(ulong id) // parameters?
        {
            // initialize instance variables & other setup
            snapshots = new List<List<KinectPoint>>();
            currentFrame = 0;
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
            List < KinectPoint > currShot = snapshots[currentFrame];
            Console.WriteLine(currShot.Count);
            GL.Enable(EnableCap.DepthTest);
            GL.PointSize(1.0f);
            GL.Begin(PrimitiveType.Points);
            foreach(KinectPoint kp in currShot)
            {
                    GL.Color4(kp.color);
                    GL.Vertex3(kp.p);
            }
            GL.End();

            currentFrame++;
        }

    }
}