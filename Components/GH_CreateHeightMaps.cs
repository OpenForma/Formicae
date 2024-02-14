using System;
using System.Collections.Generic;
using Formicae.Types;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Formicae.Components
{
    public class GH_CreateHeightMaps : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_CreateHeightMaps class.
        /// </summary>
        public GH_CreateHeightMaps()
          : base("GH_CreateHeightMaps", "Nickname",
              "Description",
              "Formicae", "Setup")

        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("WindSimModel", "WindSimModel", "WindSimModel", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("HeightsMapWithoutBuildings", "HeightsMapWithoutBuildings", "HeightsMapWithoutBuildings", GH_ParamAccess.list);
            pManager.AddNumberParameter("HeightsMapWithBuildings", "HeightsMapWithBuildings", "HeightsMapWithBuildings", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            WindSimulationModel windSimModel = new WindSimulationModel();
            DA.GetData(0,ref windSimModel);

            HeightMap heightMap = new HeightMap(windSimModel);

            //var mappedWithoutBuildings = HeightMap.MapToDomain(heightMap.HeightMapWithoutBuildings(), 0, 255);
            //var mappedWithBuildings = HeightMap.MapToDomain(heightMap.HeightMapWithBuildings(), 0, 255);

            var mappedWithoutBuildings = heightMap.HeightMapWithoutBuildings();
            var mappedWithBuildings = heightMap.HeightMapWithBuildings();

            //DA.SetDataList(0,heightMap.HeightMapWithoutBuildings());
            //DA.SetDataList(1,heightMap.HeightMapWithBuildings());


            DA.SetDataList(0, mappedWithoutBuildings);
            DA.SetDataList(1, mappedWithBuildings);

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
            get { return new Guid("0B9A9778-22FA-49AA-9C3E-730022BB56AD"); }
        }
    }
}