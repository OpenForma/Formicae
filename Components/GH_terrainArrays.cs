using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;

using Grasshopper.Kernel.Types;
using Newtonsoft.Json;
using static Formicae.Helpers.RayTracing;
using static Formicae.Helpers.MeshHelper;

using System.Drawing;
using System.IO.Compression;
using System.IO;
using System.Text;
namespace Formicae.Components
{
    public class GH_terrainArrays : GH_Component
    {
        public override Guid ComponentGuid => new Guid("792CBE8E-4903-4F5F-A408-8087DC7FA6CB");

        protected override Bitmap Icon => null;

        public IGH_GeometricGoo terrain;

        public GH_terrainArrays()
          : base("GH_terrainArrays", "FG_TA",
              "GH_terrainArrays",
              Config.FormicaeTab, Config.Tabs.Geometry)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Terrain", "T", "", GH_ParamAccess.item);
            pManager.AddGeometryParameter("Terrain + Building", "T+B", "", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Height Dictionary", "HeightDict", "HeightDict", GH_ParamAccess.item);
            pManager.AddMeshParameter("Analysis Mesh", "AnalysisMesh", "AnalysisMesh", GH_ParamAccess.item);
            pManager.AddGenericParameter("Debug", "Debug", "Debug", GH_ParamAccess.item);

        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Rhino.Geometry.Mesh terrainAndBuildings = new Rhino.Geometry.Mesh();
            DA.GetData(0, ref terrain);
            DA.GetData(1,   ref terrainAndBuildings);

            var terrainToMesh = Remesh(terrain);
            var terrainAndBuildingsToMesh = terrainAndBuildings;

            var rectTerrain = GetBase(terrainToMesh);
            
            var gridPoints = GetPoints(rectTerrain);
            var analysisPoints = GetPointsAnalysis(rectTerrain);

            var rayTerrain = HitPointsHeight(gridPoints, terrainToMesh);
            Rhino.Geometry.Mesh ProbMesh = CreateMeshFromGridPoints(HitPoints(analysisPoints, terrainToMesh),201,201);
            //Rhino.Geometry.Mesh ProbMesh = CreateMeshFromGridPoints(HitPoints(analysisPoints, terrainToMesh), 200, 200);


            var rayTerrainAndBuilding = HitPointsHeight(gridPoints, terrainAndBuildingsToMesh);

            double apiMin = rayTerrainAndBuilding.Min();
            double apiMax = rayTerrain.Max();

            int[] apiBuildingParallel = new int[rayTerrainAndBuilding.Length];
            int[] apiTerrainParallel = new int[rayTerrain.Length];


            if (apiMax != apiMin)
            {
                for (int index = 0; index < rayTerrainAndBuilding.Length; index++)
                {
                    var value = rayTerrainAndBuilding[index];
                    var normalizedValue = (value - apiMin) / (apiMax - apiMin) * 255;
                    apiBuildingParallel[index] = (int)Math.Round(normalizedValue);
                }

                for (int index = 0; index < rayTerrain.Length; index++)
                {
                    var value = rayTerrain[index];
                    var normalizedValue = (value - apiMin) / (apiMax - apiMin) * 255;
                    apiTerrainParallel[index] = (int)Math.Round(normalizedValue);
                }
            }

            List<int> apiBuilding = new List<int>(apiBuildingParallel);
            List<int> apiTerrain = new List<int>(apiTerrainParallel);

            var heightMaps = new Dictionary<string, object>
            {
                 {"terrainHeightArray", apiTerrain},
                 {"minHeight", apiMin},
                 {"maxHeight", apiMax},
                 {"buildingAndTerrainHeightArray", apiBuilding}
            };

            string jsonString = JsonConvert.SerializeObject(heightMaps,Formatting.None);

            DA.SetData(0, jsonString);
            DA.SetData(1, ProbMesh);
            //DA.SetDataList(2, gridPoints);


        }

        public static byte[] CompressString(string str)
        {
            byte[] uncompressedBytes = Encoding.UTF8.GetBytes(str);

            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    gzipStream.Write(uncompressedBytes, 0, uncompressedBytes.Length);
                }

                return memoryStream.ToArray();
            }
        }

        public static string ToBase64String(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

    }
}