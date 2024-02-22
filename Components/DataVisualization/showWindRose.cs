using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using System.Drawing;
using Newtonsoft.Json;
namespace Formicae.Components.DataVisualization
{
    public class showWindRose : GH_Component
    {
        public override Guid ComponentGuid => new Guid("A45FC4A2-0E1C-415C-86D7-D29D21AD74E1");
        protected override Bitmap Icon => null;
        public showWindRose()
          : base("showWindRose", "showWindRose",
              "showWindRose",
              Config.FormicaeTab, Config.Tabs.Show)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
        }
     
    }
}