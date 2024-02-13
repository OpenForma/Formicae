using System;
using System.Collections.Generic;
using Formicae.Types;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Formicae.Components
{
    public class GH_CreateSimulationBox : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_CreateSimulationBox class.
        /// </summary>
        public GH_CreateSimulationBox()
          : base("Create Simulation Box ", "Nickname",
              "Description",
              "Formicae", "Setup")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Box", "box", "500X500 box to make the simulaiton Grids", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // pManager.AddMeshParameter("SimulaitonMeshGrid", "SMG", "A grid to simulate on", GH_ParamAccess.item);
            pManager.AddPointParameter("SimulationPoints", "Simpts", "A grid of points to get the height maps", GH_ParamAccess.list);
            pManager.AddMeshParameter("ResultMeshGrid", "RMG", "A grid to viz the results",GH_ParamAccess.item);
            pManager.AddGenericParameter("SimulationBox", "simBox", "Simulation box object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Brep box = new Brep();
            DA.GetData(0, ref box);

            SimulationBox simbox = new SimulationBox(box);
            //var simMesh = simbox.GetSimulationMesh();
            var simPts = simbox.GetSimulationPoints();
            var ResultMesh = simbox.GetResultMeshGrid();

            DA.SetDataList(0, simPts);
            DA.SetData(1, ResultMesh);
            DA.SetData(2, simbox);

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
            get { return new Guid("1F1E5EE5-6DD9-436C-9C7B-4D51AD92B912"); }
        }
    }
}