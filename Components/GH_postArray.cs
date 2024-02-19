using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Drawing;
using Newtonsoft.Json;

namespace Formicae.Components
{

    public class GH_postArray : GH_Component
    {
        public override Guid ComponentGuid => new Guid("05AACC05-6188-4760-BB01-796D6DF98607");
        protected override Bitmap Icon => null;

        public GH_postArray()
          : base("GH_postArray", "GH_postArray",
              "GH_postArray",
              Config.FormicaeTab, Config.Tabs.API)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("HeightArray", "HeightArr", "HeightArr", GH_ParamAccess.item);
            pManager.AddTextParameter("Wind Parameters", "WindPar", "WindPar", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter("PostArray", "PostArr", "PostArr", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {

            string heightArrayJson = string.Empty;
            string windParamsJson = string.Empty;
            if (!DA.GetData(0, ref heightArrayJson)) return;
            if (!DA.GetData(1, ref windParamsJson)) return;

            var windParams = JsonConvert.DeserializeObject<Dictionary<string, object>>(windParamsJson);
            object roughness = windParams.TryGetValue("roughness", out roughness) ? roughness : 0;
            windParams.Remove("roughness");
            var heightMaps = JsonConvert.DeserializeObject<Dictionary<string, object>>(heightArrayJson);

            var body = new Dictionary<string, object>
            {
                {"heightMaps", heightMaps}, 
                {"windRose", windParams},
                {"type", "comfort"},
                {"roughness", roughness},
                {"comfortScale", "lawson_lddc"}
            };
            string bodyJson = JsonConvert.SerializeObject(body); 
            DA.SetData(0, bodyJson);
        }

    }
}