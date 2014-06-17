// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/exts/land-use/trunk/

using Landis.Core;

namespace Landis.Extension.LandUse
{
    public class Main
        : Landis.Core.ExtensionMain
    {
        public static readonly ExtensionType ExtType = new ExtensionType("disturbance:land use");
        public static readonly string ExtensionName = "Land Use";

        private static ICore modelCore;
        private Parameters parameters;

        //---------------------------------------------------------------------

        public Main()
            : base(ExtensionName, ExtType)
        {
        }

        //---------------------------------------------------------------------

        public override void LoadParameters(string dataFile,
                                            ICore mCore)
        {
            modelCore = mCore;
            modelCore.UI.WriteLine("  Loading parameters from {0}", dataFile);
            ParameterParser parser = new ParameterParser();
            parameters = Landis.Data.Load<Parameters>(dataFile, parser);
        }

        //---------------------------------------------------------------------

        public override void Initialize()
        {
            modelCore.UI.WriteLine("Initializing {0}...", Name);
            Timestep = parameters.Timestep;
        }

        //---------------------------------------------------------------------

        public override void Run()
        {
            modelCore.UI.WriteLine("  (in Run method)");
        }
    }
}
