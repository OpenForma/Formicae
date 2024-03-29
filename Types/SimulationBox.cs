﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GH_IO.Serialization;
using Rhino;
using System.Runtime.InteropServices.WindowsRuntime;
using Formicae.Helpers;




namespace Formicae.Types
{
    /// <summary>
    /// A box that is used to orient and simulate the geometry 
    /// </summary>
    public class SimulationBox : IGH_Goo,IGH_GeometricGoo,IGH_PreviewData
    {

        #region Properties
        public BoundingBox BoundingBox { get; set; }  
        public Brep BoundingBoxBrep { get; set; }


        public bool IsValid => this.IsABox();

        public string IsValidWhyNot => throw new NotImplementedException();

        public string TypeName => "SimulationBox";

        public string TypeDescription => "A bounding box used to extract the simulation grid for wind analysis from Forma.";

        public BoundingBox Boundingbox => throw new NotImplementedException();

        public Guid ReferenceID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsReferencedGeometry => this.BoundingBoxBrep != null;

        public bool IsGeometryLoaded => this.BoundingBoxBrep != null;

        public BoundingBox ClippingBox => BoundingBoxBrep?.GetBoundingBox(false) ?? BoundingBox.Unset;

        public double ResultMeshFaceCount = 200; // 300 in meters

        public double SimulationGridFaceCount = 500; // 750 in meters

        public double SimulationGridResolution = 1.5;

        public double BoxHeight => GetBoxHeight();

        public double SimulationGridDistance = 750;

        public double ResultGridDistance = 300;

        /// <summary>
        /// Points that can be used to calculate height always heigher than the Simulation Box Brep
        /// </summary>
        public List<Point3d> LiftedPts => GetSimulationPointsUp();

        /// <summary>
        /// Result mesh that is heigher than the Simulation Box, Can be used to drape
        /// </summary>
        public Mesh LiftedResultMesh => GetResultMeshUp();


        #endregion

        #region constructors 

        public SimulationBox()
        {
            
        }

        /// <summary>
        /// Create a simulation bounding box based on BB.
        /// </summary>
        /// <param name="box"></param>
        public SimulationBox(BoundingBox box)
        {
            this.BoundingBox = box;
            this.BoundingBoxBrep = box.ToBrep();

        }

        /// <summary>
        /// Createa a simulation bounding box based on a brep
        /// </summary>
        /// <param name="box"></param>
        public SimulationBox (Brep box ) 
        {
            this.BoundingBoxBrep = box;
        }

        #endregion

        #region IGH_Goo

        public IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public IGH_GooProxy EmitProxy()
        {
            throw new NotImplementedException();
        }

        public bool CastFrom(object source)
        {
            throw new NotImplementedException();
        }

        public bool CastTo<T>(out T target)
        {
            throw new NotImplementedException();
        }

        public object ScriptVariable()
        {
            throw new NotImplementedException();
        }

        public bool Write(GH_IWriter writer)
        {
            throw new NotImplementedException();
        }

        public bool Read(GH_IReader reader)
        {
            throw new NotImplementedException();

        }

        #endregion

        #region IGH_GeometricGoo

        public IGH_GeometricGoo DuplicateGeometry()
        {
            return this;
        }

        public BoundingBox GetBoundingBox(Transform xform)
        {
            throw new NotImplementedException();
        }

        public IGH_GeometricGoo Transform(Transform xform)
        {
            throw new NotImplementedException();
        }

        public IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            throw new NotImplementedException();
        }

        public bool LoadGeometry()
        {
            throw new NotImplementedException();
        }

        public bool LoadGeometry(RhinoDoc doc)
        {
            throw new NotImplementedException();
        }

        public void ClearCaches()
        {
            throw new NotImplementedException();
        }


        #endregion

        #region IGH_PreviewData

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            args.Pipeline.DrawDottedPolyline(GetResultMeshOutline(), System.Drawing.Color.Magenta,true);
            args.Pipeline.Draw3dText("Interest Area", System.Drawing.Color.Black, this.GetTopCenterPlane(), 3, "Arial");

        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
            // throw new NotImplementedException();
            args.Pipeline.DrawDottedPolyline(GetResultMeshOutline(), System.Drawing.Color.Magenta, true);
            args.Pipeline.Draw3dText("Interest Area", System.Drawing.Color.Black, this.GetTopCenterPlane(), 3, "Arial");
        }

        #endregion

        #region Simulation grid

        /// <summary>
        /// Checks if the brep is a box based on the number of points 
        /// </summary>
        /// <returns>True if the brep has only 8 points</returns>
        public bool IsABox()
        {
            return this.BoundingBoxBrep.Vertices.Count == 8;
        }


        /// <summary>
        /// Get an oriented plane at the lowest point of the bounding box 
        /// </summary>
        /// <returns></returns>
        public Plane  GetGridPlaneForBotLeftCorner()
        {
            var Brepvertices = this.BoundingBoxBrep.Vertices;
            var BrepPts = Brepvertices.Select(a => a.Location).ToList();
            var LowestX = Brepvertices.Select(a => a.Location).Select(b => b.X).Min();
            var LowestY = Brepvertices.Select(a => a.Location).Select(b => b.Y).Min();
            var LowestZ = Brepvertices.Select(a => a.Location).Select(b => b.Z).Min();
            var MaxX = Brepvertices.Select(a => a.Location).Select(b => b.X).Max();
            var MaxY = Brepvertices.Select(a => a.Location).Select(b => b.Y).Max();

            Point3d Pa = new Point3d();
            Point3d Pb = new Point3d();
            Point3d Pc = new Point3d();

            foreach (var pt in BrepPts)
            {
                if (pt.X == LowestX && pt.Y == LowestY && pt.Z == LowestZ)
                {
                    Pa = pt;
                }
                if (pt.X == MaxX && pt.Y == LowestY && pt.Z == LowestZ)
                {
                    Pb = pt;
                }
                if (pt.X == LowestX && pt.Y == MaxY && pt.Z == LowestZ)
                {
                    Pc = pt;
                }
            }

            Vector3d u = Pb - Pa;
            u.Unitize();
            Vector3d v = Pc - Pa;
            v.Unitize();
            Point3d origin = Pa;

            Plane plane = new Plane(origin, u, v);
            return plane;

        }


        /// <summary>
        /// Get the result plane to generate the grid for the result grid
        /// </summary>
        /// <returns></returns>
        public Plane GetResultPlane()
        {
            var botleftcorner = GetGridPlaneForBotLeftCorner();
           //double leftoverspace = (this.SimulationGridFaceCount - this.ResultGridDistance) / 2;
            botleftcorner.Translate(botleftcorner.XAxis * 225);
            botleftcorner.Translate(botleftcorner.YAxis * 525);
            return botleftcorner; 
        }

        /// <summary>
        /// Gets the simulation Plane to generate the simulaiton grid
        /// </summary>
        /// <returns></returns>
        public Plane GetSimulationPlane()
        {
            var botleftcorner = GetGridPlaneForBotLeftCorner();
            botleftcorner.Translate(botleftcorner.YAxis * this.SimulationGridDistance);
            return botleftcorner;
        }

        ///// <summary>
        ///// Gets the grid for the result to be colored
        ///// </summary>
        ///// <returns></returns>
        //public Mesh GetResultMeshGrid () 
        //{
        //   return MeshHelper.GetGridMeshForResult(GetResultPlane(), this.ResultGridDistance,1.5);
        //}



        /// <summary>
        /// Gets the grid for the result to be colored
        /// </summary>
        /// <returns></returns>
        public Mesh GetResultMeshGrid()
        {
            return MeshHelper.GetResultMeshRowMajor(GetResultPlane(), this.ResultMeshFaceCount, this.SimulationGridResolution); //Same resolution
        }

        [Obsolete]
        /// <summary>
        /// TOO SLOW!! Get the grid for the simulation to calcualte height maps
        /// </summary>
        /// <returns></returns>
        public Mesh GetSimulationMesh()
        {
            return MeshHelper.GetGridMeshForSimulation(GetSimulationPlane(), this.SimulationGridFaceCount);
        }


      

        public List<Point3d> GetSimulationPoints()
        {
           return MeshHelper.GetGridPointsForSimulation(GetSimulationPlane(),this.SimulationGridFaceCount, this.SimulationGridResolution);
        }



        #endregion

        #region Methods

        public override string ToString()
        {
            return $"A {TypeName} with a simulation grid of size {this.SimulationGridFaceCount} * {this.SimulationGridFaceCount} of resolution {this.SimulationGridResolution} (Distance between points)" +
                $"\nAt the center of which an interest area grid (resultGrid) of size {this.ResultMeshFaceCount} * {this.ResultMeshFaceCount} of resolution {this.SimulationGridResolution} ";
        }

        /// <summary>
        /// Gets the center point in the middle of the top Surface
        /// </summary>
        /// <returns></returns>
        public Point3d GetTopCenterPoint()
        {
           return this.BoundingBox.GetCorners()[4] + this.BoundingBox.GetCorners()[7] / 2;
        }

        /// <summary>
        /// Gets the result mesh outline for Viz
        /// </summary>
        /// <returns></returns>
        public Polyline GetResultMeshOutline()
        {
            var plane = GetGridPlaneForBotLeftCorner();
            plane.Origin = GetTopCenterPoint();
            Rectangle3d rect = new Rectangle3d(plane, this.ResultMeshFaceCount, this.ResultMeshFaceCount);
            return rect.ToPolyline();
        }

        /// <summary>
        /// Gets a plane in the center of the top surface 
        /// </summary>
        /// <returns></returns>
        public Plane GetTopCenterPlane()
        {
            var plane = GetGridPlaneForBotLeftCorner();
            plane.Origin = GetTopCenterPoint();
            return plane;
        }

        public double GetBoxHeight()
        {
            // return this.BoundingBox.GetCorners()[7].Z - this.BoundingBox.GetCorners()[0].Z;
            var Brepvertices = this.BoundingBoxBrep.Vertices;
            var LowestZ = Brepvertices.Select(a => a.Location).Select(b => b.Z).Min();
            var MaxZ = Brepvertices.Select(a => a.Location).Select(b => b.Z).Max();
            return MaxZ - LowestZ;
        }


        public List<Point3d> GetSimulationPointsUp()
        {
            var lowSimPts = GetSimulationPoints();
            List<Point3d> LiftedSimPts = new List<Point3d>();
            foreach (var simPt in lowSimPts)
            {
                LiftedSimPts.Add(simPt + Vector3d.ZAxis * this.BoxHeight * 1.1);
            }
            return LiftedSimPts;
        }

        /// <summary>
        /// Put the result mesh heigher than the Simulation Box 
        /// </summary>
        /// <returns></returns>
        public Mesh GetResultMeshUp() 
        {
            Mesh mesh = GetResultMeshGrid();
            mesh.Translate(Vector3d.ZAxis * this.BoxHeight * 1.1);
            return mesh;
        }

        #endregion
    }
}
