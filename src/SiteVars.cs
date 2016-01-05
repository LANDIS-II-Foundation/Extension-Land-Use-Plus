// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   https://github.com/LANDIS-II-Foundation/Extension-Land-Use-Change

using Landis.Core;
using Landis.SpatialModeling;

namespace Landis.Extension.LandUse
{
    public class SiteVars
    {
        private static ISiteVar<LandUse> landUse;
        private static AllowHarvestSiteVar allowHarvest;

        //---------------------------------------------------------------------

        public static void Initialize(ICore modelCore)
        {
            landUse = modelCore.Landscape.NewSiteVar<LandUse>();
            allowHarvest = new AllowHarvestSiteVar();
            Model.Core.RegisterSiteVar(allowHarvest, "LandUse.AllowHarvest");
            Landis.Library.BiomassHarvest.SiteVars.CohortsPartiallyDamaged = modelCore.Landscape.NewSiteVar<int>();
        }

        //---------------------------------------------------------------------

        public static ISiteVar<LandUse> LandUse
        {
            get {
                return landUse;
            }
        }
    }
}
