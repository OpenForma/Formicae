using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry.Intersect;
using Formicae.Helpers;

namespace Formicae.Types
{
    public class HeightMap
    {
        public WindSimulationModel Model { get; set; }  

        public HeightMap() { }  

        public HeightMap(WindSimulationModel model) 
        {
            this.Model = model; 
        }

        public List<double> HeightMapWithoutBuildings()
        {
            List<double> heights = new List<double>();
           List<Point3d> projectedPts = MeshHelper.ProjectPointsDownardOnMesh(Model.SimulationBox.LiftedPts, Model.GetModelMeshWithoutBuildings());
            foreach (Point3d p in projectedPts)
            {
                heights.Add(p.Z);
            }
            return heights;
        }

        public List <double> HeightMapWithBuildings() 
        {
            List<double> heights = new List<double>();
            List<Point3d> projectedPts = MeshHelper.ProjectPointsDownardOnMesh(Model.SimulationBox.LiftedPts, Model.GetModelMeshWithBuildings());
            foreach (Point3d p in projectedPts)
            {
                heights.Add(p.Z);
            }
            return heights;
        }

        public static List<double> MapToDomain(List<double> values, double domainMin, double domainMax)
        {
            List<double> mappedValues = new List<double>();
            double range = domainMax - domainMin;

            foreach (var value in values)
            {
                double mappedValue = ((value - values.Min()) / (values.Max() - values.Min())) * range + domainMin;
                mappedValues.Add(mappedValue);
            }

            return mappedValues;
        }
    }
}
