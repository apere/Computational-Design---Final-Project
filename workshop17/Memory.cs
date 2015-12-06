using System;
using Microsoft.Kinect;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workshop17
{
    public class Memory
    {
        DateTime timeCreated;
        Vector4 Location;
        int id;
        //List<images> frames; // each index represents a collection of snapshots or photos taken of the person
        int currentFrame; // the index of the snapshot that should be displayed next time render is called
       
        // Constructor
        public Memory() // parameters?
        {
           // initialize instance variables & other setup
        }

        // Function add
        //  This function is called to add a ‘snap shot’ to our array of frames. This should only be called from a person object.
        // Parameters
        //  image - an image that we want to add to this memory
        public void add() // parameters?
        {

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
            // must return a person
            return persons[0];
        }

        // Function getOrientation
        //
        // Return
        //  A Vector pointing in the direction that we want to face
        public Vector4 getOrientation()
        {
            return new Vector4();
        }

        // Function render
        // This function should be called every time we want to display and update the memory. 
        // This should calculate the correct orientation and display the current frame.
        public void render()
        {

        }

    }
}