using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace Formicae
{
    public class FormicaeInfo : GH_AssemblyInfo
    {
        public override string Name => "Formicae";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("de566a89-9626-4ad1-8db2-4293ec8c30cc");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}