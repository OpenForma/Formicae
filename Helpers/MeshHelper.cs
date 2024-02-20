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

        /// <summary>
        /// Get the rectangle to generate the grids 
        /// The rectangle is 750 X 750 
        /// https://app.autodeskforma.com/forma-embedded-view-sdk/docs/types/predictive_analysis.HeightMaps.html
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Rectangle3d GetBase(Rhino.Geometry.Mesh mesh)
        {
            BoundingBox box = mesh.GetBoundingBox(false);

            double zOffset = 100;
            Point3d center = new Point3d((box.Min.X + box.Max.X) / 2, (box.Min.Y + box.Max.Y) / 2, box.Max.Z + zOffset);
            double length = 750;
            double width = 750;
            Point3d rectStart = new Point3d(center.X - length / 2, center.Y - width / 2, center.Z);
            Point3d rectEnd = new Point3d(center.X + length / 2, center.Y + width / 2, center.Z);
            Rectangle3d rect = new Rectangle3d(new Plane(center, Vector3d.ZAxis), rectStart, rectEnd);

            return rect;
        }

        /// <summary>
        /// Gets the points for the large grid
        /// </summary>
        /// <param name="rect">Base rectangle</param>
        /// <returns></returns>
        public static Point3d[] GetPoints(Rectangle3d rect)
        {
            int divisions = 500;
            double spacingX = rect.Width / (divisions - 1);
            double spacingY = rect.Height / (divisions - 1);

            int arrayLength = divisions * divisions;
            Point3d[] points = new Point3d[arrayLength];
            for (int i = 0; i < divisions; i++)
            {
                for (int j = 0; j < divisions; j++)
                {
                    double x = rect.Corner(0).X + (j * spacingX);
                    double y = rect.Corner(0).Y + (i * spacingY);
                    Point3d pt = new Point3d(x, y, rect.Corner(0).Z);
                    int index = (i * divisions) + j;
                    points[index] = pt;
                }
            }

            return points;
        }

        /// <summary>
        /// Get the analysis points for the interest area
        /// https://app.autodeskforma.com/forma-embedded-view-sdk/docs/types/predictive_analysis.HeightMaps.html
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Point3d[] GetPointsAnalysis(Rectangle3d rect)
        {
            Point3d center = new Point3d(
              (rect.Corner(0).X + rect.Corner(2).X) / 2,
              (rect.Corner(0).Y + rect.Corner(2).Y) / 2,
              rect.Corner(0).Z);

            double desiredLength = 300; // Total length in x
            double desiredWidth = 300; // Total length in y
            int desiredPointsX = 201; // Desired number of points in x
            int desiredPointsY = 201; // Desired number of points in y

            double spacingX = desiredLength / (desiredPointsX - 1); // Spacing between points in x
            double spacingY = desiredWidth / (desiredPointsY - 1); // Spacing between points in y

            int divisionsX = desiredPointsX; // Number of divisions in x
            int divisionsY = desiredPointsY; // Number of divisions in y

            double actualLength = spacingX * (divisionsX - 1); // Actual total length in x
            double actualWidth = spacingY * (divisionsY - 1); // Actual total length in y

            Point3d newRectStart = new Point3d(center.X - actualLength / 2, center.Y - actualWidth / 2, center.Z);
            Point3d newRectEnd = new Point3d(center.X + actualLength / 2, center.Y + actualWidth / 2, center.Z);
            Rectangle3d newRect = new Rectangle3d(new Plane(center, Vector3d.ZAxis), newRectStart, newRectEnd);

            Point3d[] points = new Point3d[desiredPointsX * desiredPointsY];

            for (int i = 0; i < divisionsY; i++)
            {
                for (int j = 0; j < divisionsX; j++)
                {
                    double x = newRect.Corner(0).X + (j * spacingX);
                    double y = newRect.Corner(0).Y + (i * spacingY);
                    points[i * divisionsX + j] = new Point3d(x, y, newRect.Corner(0).Z);
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
