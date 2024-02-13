using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GH_IO.Serialization;
using Rhino;
using System.Runtime.InteropServices.WindowsRuntime;


namespace Formicae.Helpers
{
    public static class MeshHelper
    {

        /// <summary>
        /// Creates a single face with squared offset (read vertically) 
        /// </summary>
        /// <param name="plane">Bot left</param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Mesh GetSingleMeshForResult(Plane plane)
        {
            // 1 meter
            double offset = 1;
            Mesh m = new Mesh();
            m.Vertices.Add(plane.Origin);
            m.Vertices.Add(plane.Origin + plane.YAxis * offset);
            m.Vertices.Add(plane.Origin + plane.YAxis * offset + plane.XAxis * offset);
            m.Vertices.Add(plane.Origin + plane.XAxis * offset);
            m.Faces.AddFace(0, 1, 2, 3);
            return m;
        }

        /// <summary>
        /// Create a mesh grid (Read vertically) 
        /// </summary>
        /// <param name="originPlane">Result Plane</param>
        /// <param name="gridTotalDistance"> 200 X 200 300 X 300 Grid etc.. </param>
        public static Mesh GetGridMeshForResult(Plane originPlane, double gridTotalDistance)
        {
            Mesh resultGrid = new Mesh();
            for (int i = 0; i < gridTotalDistance; i++)
            {
                //Create plane in x direction
                Plane offsetedPlane = originPlane;
                offsetedPlane.Translate(originPlane.XAxis * i);
                Mesh OffsetMeshinX = GetSingleMeshForResult(offsetedPlane);

                for (int j = 0; j < gridTotalDistance; j++)
                {
                    //Translate in y direction
                    Mesh tempMesh = OffsetMeshinX.DuplicateMesh();
                    tempMesh.Translate(originPlane.YAxis * j);
                    resultGrid.Append(tempMesh);
                }
            }
            return resultGrid;
        }

        /// <summary>
        /// Create a mesh grid (Read vertically) 
        /// </summary>
        /// <param name="plane">Top left corner</param>
        /// <returns></returns>
        public static Mesh GetPlaneForMeshSimulation(Plane plane) 
        {            // 1 meter
            double offset = 1;
            Mesh m = new Mesh();
            m.Vertices.Add(plane.Origin);
            m.Vertices.Add(plane.Origin - plane.YAxis * offset);
            m.Vertices.Add(plane.Origin - (plane.YAxis * offset) + (plane.XAxis * offset));
            m.Vertices.Add(plane.Origin + plane.XAxis * offset);
            m.Faces.AddFace(0, 1, 2, 3);
            return m;
        }

        [Obsolete]
        public static Mesh GetGridMeshForSimulation(Plane OriginPlane, double GridtTotalDistance)
        {
            //Too slow 
            Mesh resultGrid = new Mesh();

            for (int i = 0; i < GridtTotalDistance; i++)
            {
                //Create plane in Y direction
                Plane offsetedPlane = OriginPlane;
                offsetedPlane.Translate(OriginPlane.XAxis * i);
                Mesh OffsetMeshinY = GetPlaneForMeshSimulation(offsetedPlane);

                for (int j = 0; j < GridtTotalDistance; j++)
                {
                    //Translate in X direction
                    Mesh tempMesh = OffsetMeshinY.DuplicateMesh();
                    tempMesh.Translate(OriginPlane.XAxis * j);
                    resultGrid.Append(tempMesh);
                }
            }
            return resultGrid; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OriginPlane"></param>
        /// <param name="GridtTotalDistance"></param>
        /// <param name="gridResolution"></param>
        /// <returns></returns>
        public static List<Point3d> GetGridPointsForSimulation(Plane OriginPlane, double GridtTotalDistance , double gridResolution)
        {
            var pts = new List<Point3d>();

            for (int i = 0; i < GridtTotalDistance; i++)
            {

                //Create plane in Y direction
                Plane offsetedPlane = OriginPlane;
                offsetedPlane.Translate(OriginPlane.YAxis * -i);
                Point3d pointToOffset = offsetedPlane.Origin - OriginPlane.YAxis * gridResolution / 2;
                //Create plane in Y direction
                //Point3d pointToOffset = OriginPlane.Origin - (OriginPlane.YAxis) * gridResolution / 2*i;

                for (int j = 0; j < GridtTotalDistance; j++)
                {
                    //Translate in X direction
                    Point3d tempPt = new Point3d(pointToOffset) + (OriginPlane.XAxis) * gridResolution / 2; // move the point to the right 
                    tempPt = tempPt + OriginPlane.XAxis * gridResolution * j; // move the point with the array 
                    //Point3d tempPt = new Point3d(pointToOffset) + OriginPlane.XAxis * gridResolution * j ;
                    pts.Add(tempPt);
                }
            }

            return pts;
        }
    }
}
