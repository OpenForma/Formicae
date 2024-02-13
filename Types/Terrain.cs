using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formicae.Types
{
    public  class Terrain
    {
        public Mesh terrainMesh; 
        public Terrain() { }
        public Terrain(Mesh mesh) 
        {
            terrainMesh = mesh; 
        }

    }
}
