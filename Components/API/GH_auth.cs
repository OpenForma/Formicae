using System;

using Newtonsoft.Json.Linq;
using Grasshopper.Kernel;
using System.Threading.Tasks;
using Formicae.Helpers.API;
namespace Formicae.Components.API
{
    public class GH_auth : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public GH_auth()
          : base("auth", "auth",
              "auth",
             Config.FormicaeTab, Config.Tabs.API)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Trigger", "T", "Set to true to start the authentication process.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("AccessToken", "Token", "The OAuth access token.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool trigger = false;
            if (!DA.GetData(0, ref trigger) || !trigger)
                return;

            Task.Run(() =>
            {
                try
                {
                    // Asynchronously calling GetAccessToken
                    var accessTokenTask = OAuthHandler.GetAccessToken();
                    accessTokenTask.Wait(); // This still blocks this thread, but it's a background thread now.
                    var response = accessTokenTask.Result;

                    JObject jsonResponse = JObject.Parse(response);
                    string accessToken = jsonResponse["access_token"].ToString();

                    // Use Rhino's UI thread to safely update DA
                    Rhino.RhinoApp.InvokeOnUiThread((Action)(() =>
                    {
                        DA.SetData(0, accessToken);
                    }));
                }
                catch (Exception ex)
                {
                    Rhino.RhinoApp.InvokeOnUiThread((Action)(() =>
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error during authentication: " + ex.Message);
                    }));
                }
            });

            // Reset the trigger to false to prevent re-triggering without user input.
            trigger = false;
        }

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
            get { return new Guid("DA2D7364-F925-49BF-B044-2C9F3FA74D12"); }
        }
    }
}