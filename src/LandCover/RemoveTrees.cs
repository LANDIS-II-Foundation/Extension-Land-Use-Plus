// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/exts/land-use/trunk/

using Landis.Library.Harvest;
using Landis.SpatialModeling;

namespace Landis.Extension.LandUse.LandCover
{
    class RemoveTrees
        : AgeCohortHarvest, IChange
    {
        public const string TypeName = "RemoveTrees";

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
            CurrentSite = site;
            Cut(site);
        }
    }
}
