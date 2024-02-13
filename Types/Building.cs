using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formicae.Types
{
    public class Building
    {
        public Brep buildingBrep;

        public Building() { }
         
        public Building(Brep brep) 
        {   
            buildingBrep = brep;
        }

    }
}
