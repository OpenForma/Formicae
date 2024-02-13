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

        public static Mesh GetGridMeshForSimulation(Plane OriginPlane, double GridtTotalDistance)
        {
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
    }
}
