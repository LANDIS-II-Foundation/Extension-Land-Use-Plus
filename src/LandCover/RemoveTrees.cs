// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/exts/land-use/trunk/

using Landis.Library.Harvest;
using Landis.SpatialModeling;
using log4net;

namespace Landis.Extension.LandUse.LandCover
{
    class RemoveTrees
        : AgeCohortHarvest, IChange
    {
        public const string TypeName = "RemoveTrees";
        private static readonly ILog log = LogManager.GetLogger(typeof(RemoveTrees));
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;

        //---------------------------------------------------------------------

        string IChange.Type
        {
            get { return TypeName; }
        }

        //---------------------------------------------------------------------

        public RemoveTrees(ICohortSelector cohortSelector)
            : base(cohortSelector)
        {
            base.Type = Main.ExtType;
        }

        //---------------------------------------------------------------------

        public void ApplyTo(ActiveSite site)
        {
            if (isDebugEnabled)
                log.DebugFormat("    Applying LCC {0} to site {1}",
                                GetType().Name,
                                site.Location);
            CurrentSite = site;
            Cut(site);
        }
    }
}
