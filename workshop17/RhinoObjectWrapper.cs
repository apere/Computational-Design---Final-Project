using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using Rhino.Geometry;

namespace workshop17
{
    public class RhinoObjectWrapper
    {
        virtual public void Render()
        {
        }

        virtual public void Destroy()
        {
        }
    }


    public class RhinoCurveGL : RhinoObjectWrapper
    {
        public RhinoCurveGL(Curve _c, double _DivisionLength, int _MaxPoints)
        {
            RhinoCurve = _c;
            dlength = _DivisionLength;
            maxcount = _MaxPoints;
            UpdatePolyline();

        }


        public Curve RhinoCurve;
        public Polyline PolylineOfCurve;

        float[] vertexnbuffer;

        uint VBOid = 0;
        double dlength = 0.1;
        int maxcount = 50;

        public override void Destroy()
        {
            RhinoCurve = null;
            PolylineOfCurve = null;
            if (VBOid != 0) GL.DeleteBuffers(1, ref VBOid);
            VBOid = 0;

            vertexnbuffer = null;
            base.Destroy();
        }



        public void CurveToPolyline(Curve cc, ref Polyline pl)
        {
            Polyline pltemp = null;

            if (cc.IsLinear(0.001))
            {
                if (pl.Count() == 0) pl.Add(cc.PointAtStart);

                pl.Add(cc.PointAtEnd);
            }
            else if (cc.TryGetPolyline(out pltemp))
            {
                if (pl.Count() == 0) pl.Add(pltemp[0]);


                for (int i = 1; i < pltemp.Count(); ++i)
                {
                    pl.Add(pltemp[i]);
                }
            }
            else if ((cc is PolyCurve) && (cc as PolyCurve).SegmentCount > 1)
            {
                for (int i = 0; i < (cc as PolyCurve).SegmentCount; ++i)
                {
                    CurveToPolyline((cc as PolyCurve).SegmentCurve(i), ref pl);
                }
            }
            else
            {
                double t0 = cc.Domain.Min;
                double t1 = cc.Domain.Max;
                double L = GetApproxCurveLength(cc, 9);


                int divnum = (int)((L / dlength) + 1);

                if (divnum < 2) divnum = 2;
                else if (divnum > maxcount) divnum = maxcount;

                double dt = (t1 - t0) / (double)(divnum - 1.0);

                if (pl.Count() == 0) pl.Add(cc.PointAtStart);
                for (int i = 1; i < divnum; ++i)
                {
                    pl.Add(cc.PointAt(t0 + i * dt));
                }
            }
        }

        static public double GetApproxCurveLength(Curve cc, int res)
        {
            double t0 = cc.Domain.Min;
            double t1 = cc.Domain.Max;
            double L = 0.0;

            double dt = (t1 - t0) / (double)(res - 1.0);

            Point3d p0 = cc.PointAtStart;
            Point3d p1;

            for (int i = 1; i < res; ++i)
            {
                p1 = cc.PointAt(t0 + i * dt);
                L += p0.DistanceTo(p1);
                p0 = p1;
            }

            return L;
        }

        public void UpdatePolyline()
        {
            if (RhinoCurve == null) return;
            PolylineOfCurve = new Polyline();
            CurveToPolyline(RhinoCurve, ref PolylineOfCurve);
        }



        private void GenBuffers()
        {
            GL.GenBuffers(1, out VBOid);
        }

        public void UpdateVertexBuffer()
        {
            if (RhinoCurve == null) return;
            if (VBOid == 0) GenBuffers();

            if (vertexnbuffer == null || vertexnbuffer.Length != PolylineOfCurve.Count * 3)
            {
                vertexnbuffer = new float[PolylineOfCurve.Count * 3];
            }

            int k = 0;

            for (int i = 0; i < PolylineOfCurve.Count; ++i)
            {
                vertexnbuffer[k++] = (float)PolylineOfCurve[i].X;
                vertexnbuffer[k++] = (float)PolylineOfCurve[i].Y;
                vertexnbuffer[k++] = (float)PolylineOfCurve[i].Z;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexnbuffer.Length * sizeof(float)), vertexnbuffer, BufferUsageHint.StaticDraw);
            //GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, (IntPtr)(vertexnbuffer.Length * sizeof(float)), vertexnbuffer);
        }


        public override void Render()
        {
            if (RhinoCurve == null || PolylineOfCurve.Count < 2) return;
            if (VBOid == 0)
            {
                UpdateVertexBuffer();
            }

            int vertexStrideInBytes = 3 * sizeof(float);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, vertexStrideInBytes, (IntPtr)(0));


            GL.DrawArrays(PrimitiveType.LineStrip, 0, PolylineOfCurve.Count);


            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DisableClientState(ArrayCap.VertexArray);
        }
    }

    public class RhinoMeshGL : RhinoObjectWrapper
    {
        public RhinoMeshGL()
        {
        }

        public RhinoMeshGL(Mesh ms)
        {
            RhinoMesh = ms;
        }


        public override void Destroy()
        {
            rhinoMesh = null;

            if (VBOid != 0) GL.DeleteBuffers(1, ref VBOid);
            VBOid = 0;

            if (IBOid != 0) GL.DeleteBuffers(1, ref IBOid);
            IBOid = 0;

            vertexnbuffer = null;
            indexbuffer = null;

            base.Destroy();
        }

        protected Mesh rhinoMesh;

        public Mesh RhinoMesh
        {
            get
            {
                return rhinoMesh;
            }
            set
            {
                rhinoMesh = value;
                rhinoMesh.Faces.ConvertQuadsToTriangles();
                UpdateNormals();
            }
        }

        public void UpdateNormals()
        {
            if (rhinoMesh == null) return;
            rhinoMesh.FaceNormals.ComputeFaceNormals();
            rhinoMesh.Normals.ComputeNormals();
        }

        float[] vertexnbuffer;
        uint[] indexbuffer;

        uint VBOid = 0;
        uint IBOid = 0;

        private void GenBuffers()
        {
            GL.GenBuffers(1, out VBOid);
            GL.GenBuffers(1, out IBOid);
        }

        public void UpdateVertexBuffer()
        {
            if (rhinoMesh == null) return;
            if (VBOid == 0) GenBuffers();

            if (vertexnbuffer == null || vertexnbuffer.Length != rhinoMesh.Vertices.Count * 6)
            {
                vertexnbuffer = new float[rhinoMesh.Vertices.Count * 8];
            }

            int k = 0;

            for (int i = 0; i < rhinoMesh.Vertices.Count; ++i)
            {
                vertexnbuffer[k++] = rhinoMesh.Vertices[i].X;
                vertexnbuffer[k++] = rhinoMesh.Vertices[i].Y;
                vertexnbuffer[k++] = rhinoMesh.Vertices[i].Z;

                vertexnbuffer[k++] = rhinoMesh.Normals[i].X;
                vertexnbuffer[k++] = rhinoMesh.Normals[i].Y;
                vertexnbuffer[k++] = rhinoMesh.Normals[i].Z;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexnbuffer.Length * sizeof(float)), vertexnbuffer, BufferUsageHint.StaticDraw);
            //GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, (IntPtr)(vertexnbuffer.Length * sizeof(float)), vertexnbuffer);
        }

        public void UpdateIndexBuffer()
        {
            if (rhinoMesh == null) return;
            if (VBOid == 0) GenBuffers();
            if (indexbuffer == null || indexbuffer.Length != rhinoMesh.Faces.Count * 3)
            {
                indexbuffer = new uint[rhinoMesh.Faces.Count * 3];
            }

            int k = 0;
            for (int i = 0; i < rhinoMesh.Faces.Count; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    indexbuffer[k++] = (uint)rhinoMesh.Faces[i][j];
                }
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBOid);

            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, (IntPtr)(indexbuffer.Length * sizeof(uint)), indexbuffer, BufferUsageHint.StaticDraw);
        }


        public override void Render()
        {
            if (rhinoMesh == null) return;
            if (VBOid == 0)
            {
                UpdateIndexBuffer();
                UpdateVertexBuffer();
            }

            int vertexStrideInBytes = 6 * sizeof(float);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, vertexStrideInBytes, (IntPtr)(0));

            GL.EnableClientState(ArrayCap.NormalArray);
            GL.NormalPointer(NormalPointerType.Float, vertexStrideInBytes, (IntPtr)(3 * sizeof(float)));

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBOid);
            GL.DrawElements(PrimitiveType.Triangles, indexbuffer.Length, DrawElementsType.UnsignedInt, (IntPtr)(0));


            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);


            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
        }
    }
}
