using System;
using System.Collections.Generic;
using System.Linq;
using Formicae.Types;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Drawing;
namespace Formicae.Components
{
    public class GH_apiData : GH_Component
    {
        public override Guid ComponentGuid => new Guid("1F1E5EE5-6DD9-436C-9C7B-4D51AD92B912");
        protected override Bitmap Icon => null;
        public GH_apiData()
          : base("Create Simulation Box ", "Nickname",
              "Description",
              "Formicae", "Setup")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Box", "box", "750X750 box to make the simulaiton Grids", GH_ParamAccess.item);
            pManager.AddMeshParameter("TerrainMesh", "TM", "Mesh used to represent the terrain", GH_ParamAccess.item);
            pManager.AddBrepParameter("Buildings Breps", "BB", "Buildings as Breps (Kepp at volumetric level and simple boxes)", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {


            pManager.AddIntegerParameter("apiBuilding", "apiBuilding", "apiBuilding", GH_ParamAccess.list);
            pManager.AddIntegerParameter("apiTerrain", "apiTerrain", "apiTerrain", GH_ParamAccess.list);
            pManager.AddNumberParameter("apiMin", "apiMin", "apiMin", GH_ParamAccess.item);
            pManager.AddNumberParameter("apiMax", "apiMax", "apiMax", GH_ParamAccess.item);

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Parameters
            Brep box = new Brep();
            if (!DA.GetData(0, ref box)) return; 

            Mesh tMesh = new Mesh();
            DA.GetData(1, ref tMesh);
            Terrain terrain = new Terrain(tMesh);

            List<Brep> blgsBrep = new List<Brep>();
            DA.GetDataList(2, blgsBrep);

            Buildings buildings = new Buildings(blgsBrep);


            SimulationBox simbox = new SimulationBox(box);


            WindSimulationModel windSimModel = new WindSimulationModel(buildings, terrain, simbox);
            HeightMap heightMap = new HeightMap(windSimModel);

            var mappedWithoutBuildings = heightMap.HeightMapWithoutBuildings();
            var mappedWithBuildings = heightMap.HeightMapWithBuildings();

            double apiMin = mappedWithBuildings.Min();
            double apiMax = mappedWithBuildings.Max();


            // Pre-allocate space for the results to avoid concurrency issues
            int[] apiBuildingParallel = new int[mappedWithBuildings.Count];
            int[] apiTerrainParallel = new int[mappedWithoutBuildings.Count];


            // Check to avoid division by zero
            if (apiMax != apiMin)
            {
                for (int index = 0; index < mappedWithBuildings.Count; index++)
                {
                    var value = mappedWithBuildings[index];
                    var normalizedValue = (value - apiMin) / (apiMax - apiMin) * 255;
                    apiBuildingParallel[index] = (int)Math.Round(normalizedValue);
                }

                for (int index = 0; index < mappedWithoutBuildings.Count; index++)
                {
                    var value = mappedWithoutBuildings[index];
                    var normalizedValue = (value - apiMin) / (apiMax - apiMin) * 255;
                    apiTerrainParallel[index] = (int)Math.Round(normalizedValue);
                }
            }

            // Convert the array to a list if a List<double> is specifically needed
            List<int> apiBuilding = new List<int>(apiBuildingParallel);
            List<int> apiTerrain = new List<int>(apiTerrainParallel);


            DA.SetDataList(0, apiBuilding);
            DA.SetDataList(1, apiTerrain);
            DA.SetData(2, apiMin);
            DA.SetData(3, apiMax);


        }


    }
}