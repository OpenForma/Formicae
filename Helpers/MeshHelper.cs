using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Rhino.DocObjects;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GH_IO.Serialization;
using Rhino;
using System.Runtime.InteropServices.WindowsRuntime;
using Rhino.Geometry.Intersect;
using System.Drawing.Printing;
using System.Security.Policy;
using System.Collections.Concurrent;


namespace Formicae.Helpers
{
    public class MeshHelper
    {

        public static Rhino.Geometry.Mesh Remesh(IGH_GeometricGoo goo)
        {
            Guid id = goo.ReferenceID;
            var rhinoObj = new RhinoObject[] { RhinoDoc.ActiveDoc.Objects.Find(id) };
            if (rhinoObj == null) return null;

            ObjRef[] getMesh = Rhino.DocObjects.RhinoObject.GetRenderMeshesWithUpdatedTCs(rhinoObj, false, false, false, false); //hidden objects won't be ignored
            if (getMesh.Length == 0) return null;

            return getMesh[0].Mesh();
        }

        public static Rectangle3d GetBase(Rhino.Geometry.Mesh mesh)
        {
            BoundingBox box = mesh.GetBoundingBox(false);
            
            double zOffset = 100; 
            Point3d center = new Point3d((box.Min.X + box.Max.X) / 2, (box.Min.Y + box.Max.Y) / 2, box.Max.Z + zOffset);
            double length = 500; 
            double width = 500; 
            Point3d rectStart = new Point3d(center.X - length / 2, center.Y - width / 2, center.Z);
            Point3d rectEnd = new Point3d(center.X + length / 2, center.Y + width / 2, center.Z);
            Rectangle3d rect = new Rectangle3d(new Plane(center, Vector3d.ZAxis), rectStart, rectEnd);

            return rect;
        }

        public static Point3d[] GetPoints(Rectangle3d rect)
        {
            int divisions = (int)Math.Sqrt(250000) - 1; 
            double width = rect.Width;
            double height = rect.Height;
            double spacingX = width / divisions;
            double spacingY = height / divisions;

            int arrayLength = (divisions + 1) * (divisions + 1);
            Point3d[] points = new Point3d[arrayLength];
            for (int i = 0; i <= divisions; i++)
            {
                for (int j = 0; j <= divisions; j++)
                {

                    double x = rect.Corner(0).X + (j * spacingX);
                    double y = rect.Corner(0).Y + (i * spacingY);
                    Point3d pt = new Point3d(x, y, rect.Corner(0).Z);
                    int index = (i * (divisions + 1)) + j;
                    points[index] = pt;
                }
            }

            return points;
        }


        public static Point3d[] GetPointsAnalysis(Rectangle3d rect)
        {
       
            Point3d center = new Point3d(
                (rect.Corner(0).X + rect.Corner(2).X) / 2,
                (rect.Corner(0).Y + rect.Corner(2).Y) / 2,
                rect.Corner(0).Z); 

            double desiredLength = 200; 
            double desiredWidth = 200;

            Point3d newRectStart = new Point3d(center.X - desiredLength / 2, center.Y - desiredWidth / 2, center.Z);
            Point3d newRectEnd = new Point3d(center.X + desiredLength / 2, center.Y + desiredWidth / 2, center.Z);
            Rectangle3d newRect = new Rectangle3d(new Plane(center, Vector3d.ZAxis), newRectStart, newRectEnd);
            int divisions = (int)Math.Sqrt(40000); 
            double width = newRect.Width;
            double height = newRect.Height;
            double spacingX = width / divisions;
            double spacingY = height / divisions;

            Point3d[] points = new Point3d[(divisions + 1) * (divisions + 1)]; // Adjust for one extra in each dimension

            for (int i = 0; i <= divisions; i++)
            {
                for (int j = 0; j <= divisions; j++)
                {
                    double x = newRect.Corner(0).X + (j * spacingX);
                    double y = newRect.Corner(0).Y + (i * spacingY);
                    points[i * (divisions + 1) + j] = new Point3d(x, y, newRect.Corner(0).Z); // Assuming flat Z
                }
            }

            return points;
        }




        public static Mesh CreateMeshFromGridPoints(Point3d[] points, int gridWidth, int gridHeight)
        {
            if (points == null || points.Length == 0)
                throw new ArgumentException("Points array is null or empty", nameof(points));
            if (points.Length != gridWidth * gridHeight)
                throw new ArgumentException("Points array size does not match grid dimensions", nameof(points));

            Mesh mesh = new Mesh();
            foreach (Point3d point in points)
            {
                mesh.Vertices.Add(point);
            }

            // Create faces
            for (int y = 0; y < gridHeight - 1; y++)
            {
                for (int x = 0; x < gridWidth - 1; x++)
                {
                    int lowerLeft = y * gridWidth + x;
                    int lowerRight = y * gridWidth + x + 1;
                    int upperLeft = (y + 1) * gridWidth + x;
                    int upperRight = (y + 1) * gridWidth + x + 1;

                    mesh.Faces.AddFace(lowerLeft, lowerRight, upperRight, upperLeft);
                }
            }

            mesh.Normals.ComputeNormals();
            mesh.Compact(); 

            return mesh;
        }


    }
}
