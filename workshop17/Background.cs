using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using Rhino.Geometry;

namespace workshop17
{
    public class Background
    {
        //public int Width = 0;
        //public int Height = 0;
        public List<RhinoMeshGL> meshes = new List<RhinoMeshGL>();
        public List<RhinoCurveGL> curves = new List<RhinoCurveGL>();
        public RhinoMeshGL RgridMesh;

        public Background()
        {

    }


        //time variable. We'll increase its value by a small amount at each frame iteration

        double time = 0.0;

        public void OnframeUpdate()
        {
            time += 0.1; //time step
            //double viewAngle = time * 0.3;

            //................................................................................Drawing

            double a = 10.0;
            double b = 20.0;
            double c = 10.0;
            double x, y,z;
            double factor = 10.0;


            /* GL.Begin(PrimitiveType.Points);
             GL.MatrixMode();
             GL.Enable(EnableCap.DepthTest);
             GL.PointSize(25.0f);
             for(int i = 0; i < 1000; i++){
                 for(int j = 0; j < 500; j++) { 
                 x = i;
                 y = (a * (x * x) - (b * x) - c);
                 GL.Color3(0.0f, 0.0f, 0.0f);
                 GL.Vertex3(x*0.2, y*0.2, c*j*0.1);
                 }
             }*/


            List<OpenTK.Vector3d> pointsT = new List<OpenTK.Vector3d>();

            GL.Begin(PrimitiveType.Points);
            GL.Enable(EnableCap.DepthTest);
            for (float v = 0; v <= 5 * Math.PI; v=v+0.05f)
            {
                for (float u = 0; u <= Math.PI; u=u + 0.05f)
                {
                    x = u + v;
                    y = (u + Math.Sin(u + v) / 4) * Math.Cos(u);
                    z= (u+Math.Sin(4*v)/8) *Math.Sin(u);
                    GL.Color3(0.0f, 0.0f, 0.0f);
                    GL.Vertex3(x, z, y);
                    OpenTK.Vector3d p = new OpenTK.Vector3d(x, z, y);
                    pointsT.Add(p);
                }
            }
            GL.End();

            int ny = 60;//(float)(Math.PI)*(5);
            int nx = 30; //(float)Math.PI;
            Mesh gmesh;
            gmesh = new Mesh();



            for (int i = 0; i < pointsT.Count; ++i)
                {
                    gmesh.Vertices.Add(pointsT[i].X, pointsT[i].Y, pointsT[i].Z);
                }

                for (int v = 0; v <nx; v = v++)
                {
                    for (int u = 0; u <ny; u++)
                    {
                        int k = v * ny + v;
                        gmesh.Faces.AddFace(k, k + 1, k + 1 + ny);
                        gmesh.Faces.AddFace(k, k + 1 + ny, k + ny);
                    }
                }

                RgridMesh = new RhinoMeshGL(gmesh);
                meshes.Add(RgridMesh);

                /*GL.Begin(PrimitiveType.Lines);
                GL.Enable(EnableCap.DepthTest);
                for (int u = 0; u <= Math.PI; u++)
                {
                    for (int v = 0; v <= 5 * Math.PI; v++)
                    {
                        x = u + v;
                        y = (u + Math.Sin(u + v) / 4) * Math.Cos(u);
                        z = (u + Math.Sin(4 * v) / 8) * Math.Sin(u);
                        GL.Color3(0.0f, 0.0f, 0.0f);
                        GL.Vertex3(x, y, z * factor);
                    }

                }
                GL.End();
                */
                /*GL.Begin(PrimitiveType.Lines);
                GL.Enable(EnableCap.DepthTest);
                GL.LineWidth(15.0f);
                for (int i = 0; i < 1000; i++)
                {
                    for (int j = 0; j < 500; j++)
                    {
                        x = i;
                        y = (a * (x * x) - (b * x) - Math.Sin(c));
                        GL.Color3(0.0f, 0.0f, 0.0f);
                        GL.Vertex3(x , y, c * i);
                    }
                }

                GL.End();*/
                Console.WriteLine(a+" "+b + " "+c + " ");





            /*//Building a list of points that represent a grid with variable elevation z=cos(x+time)*sin*(y)
            double x0 = -30.0;
            double x1 = 30.0;
            double y0 = -16.0;
            double y1 = 16.0;

            int nx = 60;
            int ny = 40;

            double dx = (x1 - x0) / (nx - 1.0);
            double dy = (y1 - y0) / (ny - 1.0);

            List<OpenTK.Vector3d> points = new List<OpenTK.Vector3d>();

            for (int i = 0; i < nx; ++i)
            {
                for (int j = 0; j < ny; ++j)
                {

                    double x = x0 + i * dx;
                    double y = y0 + j * dy;

                    double z = Math.Cos(x + time) * Math.Sin(y);

                    OpenTK.Vector3d p = new OpenTK.Vector3d(x, y, z);

                    points.Add(p);
                }
            }




            //from PAN...
            //Render grid points
            GL.PointSize(3.0f);
            GL.Color4(0.0, 0.0, 0.0, 0.8);
            GL.Begin(PrimitiveType.Points);
            for (int i = 0; i < points.Count; ++i)
            {
                GL.Vertex3(points[i]);
            }
            GL.End();
            */
            //render a solid surface by connecting quadruplets of grid points

        /*GL.Begin(PrimitiveType.Quads);
        for (int i = 0; i < nx - 1; ++i)
        {
            for (int j = 0; j < ny - 1; ++j)
            {
                int k = i * ny + j;

                GL.Vertex3(pointsT[k]);
                GL.Vertex3(pointsT[k + 1]);
                GL.Vertex3(pointsT[k + 1 + ny]);
                GL.Vertex3(pointsT[k + ny]);
            }
        }
        GL.End();*/
        


    }
        
    }
}
