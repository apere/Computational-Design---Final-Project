using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace workshop17
{
    class Background
    {
        public int Width = 0;
        public int Height = 0;

        public void Initialize()
        {
        
        }
        //time variable. We'll increase its value by a small amount at each frame iteration
        double time = 0.0;

        public void OnframeUpdate()
        {
            time += 0.1; //time step
            double viewAngle = time * 0.3;

            //................................................................................Drawing
            //Building a list of points that represent a grid with variable elevation z=cos(x+time)*sin*(y)
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

            //render a solid surface by connecting quadruplets of grid points
            GL.Begin(PrimitiveType.Quads);
            for (int i = 0; i < nx - 1; ++i)
            {
                for (int j = 0; j < ny - 1; ++j)
                {
                    int k = i * ny + j;

                    GL.Vertex3(points[k]);
                    GL.Vertex3(points[k + 1]);
                    GL.Vertex3(points[k + 1 + ny]);
                    GL.Vertex3(points[k + ny]);
                }
            }
            GL.End();

        }
        
    }
}
