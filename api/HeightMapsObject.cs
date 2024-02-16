using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Formicae.api
{
    /// <summary>
    /// Wrapper class for getting a json string that can be used to be sent to Forma
    /// </summary>
    public class HeightMapsObject
    {
        /// <summary>
        /// Minimum height in meters of the buildings and terrain 
        /// <br>Required input for Formas wind predection</br>
        /// </summary>
        double minHeight { get; set; }
        /// <summary>
        /// Maximum height in meters of the buildings and terrain 
        /// <br>Required input for Formas wind predection</br>
        /// </summary>
        double maxHeight { get; set; }

        /// <summary>
        /// Terrain only height data remapped 0-255
        /// <br>Required input for Formas wind predection</br>
        /// </summary>
        List<int> terrainHeightArray { get; set; }

        /// <summary>
        /// Terrain and buildings height data remapped 0-255
        /// <br>Required input for Formas wind predection</br>
        /// </summary>
        List<int> buildingAndTerrainHeightArray { get; set; }   

        public HeightMapsObject() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_minHeight">Minimum height in meters of the buildings and terrain </param>
        /// <param name="_maxHeight">Maximum height in meters of the buildings and terrain </param>
        /// <param name="_terrainHeightArray">Terrain only height data remapped 0-255</param>
        /// <param name="_buildingAndTerrainHeightArray">Terrain and buildings height data remapped 0-255</param>
        public HeightMapsObject(double _minHeight, double _maxHeight, List<int> _terrainHeightArray, List<int> _buildingAndTerrainHeightArray)
        {
            this.minHeight = _minHeight;
            this.maxHeight = _maxHeight;
            this.terrainHeightArray = _terrainHeightArray;
            this.buildingAndTerrainHeightArray = _buildingAndTerrainHeightArray;
        }
            
        public string GetJsonString () 
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
