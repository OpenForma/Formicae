using System;
using System.Collections.Generic;
using Formicae.Types;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Formicae.Components
{
    public class GH_CreateWindSimulationModel : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_CreateWindSimulationModel class.
        /// </summary>
        public GH_CreateWindSimulationModel()
          : base("GH_CreateWindSimulationModel", "Nickname",
              "Description",
              "Formicae", "Setup")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("TerrainMesh", "TM", "Mesh used to represent the terrain", GH_ParamAccess.item);
            pManager.AddBrepParameter("Buildings Breps", "BB", "Buildings as Breps (Kepp at volumetric level and simple boxes)", GH_ParamAccess.list);
            pManager.AddGenericParameter("Simulation Box", "SimBox", "Simulation Box Object", GH_ParamAccess.item);
           
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("WindSimModel", "WindSimModel", "WindSimModel",GH_ParamAccess.item);
           // pManager.AddGenericParameter("Debug", "Debug", "Debug", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh tMesh = new Mesh();
            DA.GetData(0, ref tMesh);
            Terrain terrain = new Terrain(tMesh);

            List<Brep> blgsBrep = new List<Brep>(); 
            DA.GetDataList(1, blgsBrep);
            Buildings bldgs = new Buildings(blgsBrep);

            SimulationBox simBox = new SimulationBox();
            DA.GetData(2, ref simBox);

            WindSimulationModel windSimModel = new WindSimulationModel(bldgs, terrain, simBox);

            DA.SetData(0, windSimModel);
            //DA.SetData(1, simBox);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7C2E1E1E-CAB8-47B0-956B-9351A3BDB8BD"); }
        }
    }
}