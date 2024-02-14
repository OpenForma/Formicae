using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formicae.Types
{
    public class Buildings
    {
        public Brep buildingBreps;

        public Buildings() { }
        
        public Mesh previewMesh => GetBuildingsPreviewMesh();

        public Buildings(Brep brep) 
        {
            buildingBreps = brep;
        }

        public Buildings(List<Brep>breps)
        {
            Brep brep = new Brep();
            foreach(var b in breps) 
            {
                brep.Append(b);
            }
            buildingBreps = brep;
        }

        public Mesh GetBuildingsPreviewMesh() 
        {
            Mesh mesh = new Mesh();
            Mesh[] meshes = Mesh.CreateFromBrep(this.buildingBreps, MeshingParameters.FastRenderMesh);
            foreach (var m in meshes)
            {
                mesh.Append(m);
            }
            return mesh;
        }
        

    }
}
