
using Rhino.Geometry;
using DHARTAPI.Geometry;
using DHARTAPI.RayTracing;
using System.Diagnostics;
using System.Threading.Tasks;


namespace Formicae.Helpers
{
    public class RayTracing
    {
        public static Point3d[] HitPoints(Point3d[] points, Rhino.Geometry.Mesh contextMesh)
        {
            Point3d[] points_ = new Point3d[points.Length];
            MeshInfo _contextMesh = new MeshInfo(contextMesh.Faces.ToIntArray(true), contextMesh.Vertices.ToFloatArray());
            EmbreeBVH bvh = new EmbreeBVH(_contextMesh);

            var analysisPoints = new DHARTAPI.Vector3D[points.Length];
            for (int i = 0; i < points.Length; i++)
                analysisPoints[i] = new DHARTAPI.Vector3D((float)points[i].X, (float)points[i].Y, (float)points[i].Z);


            var direction_vector = new DHARTAPI.Vector3D(0, 0, -1);


            DHARTAPI.Vector3D[] hitPoints = new DHARTAPI.Vector3D[points.Length];

            Parallel.For(0, points.Length, i =>       
            {
                hitPoints[i] = EmbreeRaytracer.IntersectForPoint(bvh, analysisPoints[i], direction_vector);
                points_[i] = new Point3d(hitPoints[i].x, hitPoints[i].y, hitPoints[i].z); ;
            });

            return points_;
        }


        public static double[] HitPointsHeight(Point3d[] points, Rhino.Geometry.Mesh contextMesh)
        {
            Point3d[] points_ = new Point3d[points.Length];
            double[] height_ = new double[points.Length];

            MeshInfo _contextMesh = new MeshInfo(contextMesh.Faces.ToIntArray(true), contextMesh.Vertices.ToFloatArray());
            EmbreeBVH bvh = new EmbreeBVH(_contextMesh);


            var analysisPoints = new DHARTAPI.Vector3D[points.Length];
            for (int i = 0; i < points.Length; i++)
                analysisPoints[i] = new DHARTAPI.Vector3D((float)points[i].X, (float)points[i].Y, (float)points[i].Z);


            var direction_vector = new DHARTAPI.Vector3D(0, 0, -1);
            DHARTAPI.Vector3D[] hitPoints = new DHARTAPI.Vector3D[points.Length];
            Parallel.For(0, points.Length, i =>
            {
                
                hitPoints[i] = EmbreeRaytracer.IntersectForPoint(bvh, analysisPoints[i], direction_vector);
                points_[i] = new Point3d(hitPoints[i].x, hitPoints[i].y, hitPoints[i].z);
                height_[i] =  hitPoints[i].z; 
            });

            return height_;
        }


        public static void LogTime(ref Stopwatch sw, string text)
        {
            sw.Stop();
            Rhino.RhinoApp.WriteLine($"{text}: {sw.ElapsedMilliseconds} ms");
            sw.Restart();
        }

    }
}
