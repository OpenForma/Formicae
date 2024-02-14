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

        public List<Brep> buildings; 

        public Buildings() { }
        
        public Mesh previewMesh => GetBuildingsPreviewMesh();

        public Buildings(Brep brep) 
        {
            buildingBreps = brep;
        }

        public Buildings(List<Brep>breps)
        {
            Brep brep = new Brep();
            //foreach(var b in breps) 
            //{
            //    brep.Append(b);
            //}
            //buildingBreps = brep;

            this.buildings = breps; 
        }

        public Mesh GetBuildingsPreviewMesh() 
        {
            //Mesh mesh = new Mesh();
            //Mesh[] meshes = Mesh.CreateFromBrep(this.buildingBreps, MeshingParameters.FastRenderMesh);
            //foreach (var m in meshes)
            //{
            //    mesh.Append(m);
            //}
            //return mesh;

            Mesh mesh = new Mesh();
            foreach ( var bldg in buildings ) 
            {
                mesh.Append(Mesh.CreateFromBrep(bldg, MeshingParameters.FastRenderMesh));
            }

            return mesh;
        }
        

    }
}
