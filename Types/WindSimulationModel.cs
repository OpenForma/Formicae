using Formicae.Helpers;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formicae.Types
{
    public class WindSimulationModel
    {
        WindSimulationModel() { }
        
        public Buildings Buildings { get; set; } 
        public Terrain Terrain { get; set; }
        public SimulationBox SimulationBox { get; set; }

        public Mesh ModelWithBuildings => GetModelMeshWithBuildings();
        public Mesh ModelwithoutBuildings => GetModelMeshWithoutBuildings();

        public Mesh DrapedResultMesh => GetResultMeshDraped();

        WindSimulationModel(Buildings blgs , Terrain terrain, SimulationBox simBox) 
        {
            this.Buildings = blgs;
            this.Terrain = terrain;
            this.SimulationBox = simBox;
        }

        public Mesh GetModelMeshWithBuildings()
        {
            Mesh mesh = new Mesh();
            mesh.Append(Buildings.previewMesh);
            mesh.Append(Terrain.terrainMesh);
            return mesh;
        }

        public Mesh GetModelMeshWithoutBuildings() 
        {
            Mesh mesh = new Mesh(); 
            mesh.Append(Terrain.terrainMesh);
            return mesh;
        }

        public Mesh GetResultMeshDraped()
        {
            return MeshHelper.DrapeMesh(this.SimulationBox.LiftedResultMesh, this.Terrain.terrainMesh);
        }
       

    }
}
