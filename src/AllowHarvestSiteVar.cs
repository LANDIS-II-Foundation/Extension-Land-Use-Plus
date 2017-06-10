// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   https://github.com/LANDIS-II-Foundation/Extension-Land-Use-Change

using Landis.SpatialModeling;

namespace Landis.Extension.LandUse
{
    /// <summary>
    /// A wrapper around the LandUse site variable that exposes the 
    /// AllowHarvest property as a read-only bool site variable.
    /// </summary>
    public class AllowHarvestSiteVar
        : ISiteVar<bool>
    {
        #region ISiteVariable members
        System.Type ISiteVariable.DataType
        {
            get
            {
                return typeof(bool);
            }
        }

        InactiveSiteMode ISiteVariable.Mode
        {
            get
            {
                return SiteVars.LandUse.Mode;
            }
        }

        ILandscape ISiteVariable.Landscape
        {
            get
            {
                return SiteVars.LandUse.Landscape;
            }
        }
        #endregion

        #region ISiteVar<bool> members
        // Other extensions only need read access.
        // We can add write-access to the AllowHarvest property by creating a setter modifying the field.
        // Discover: AllowHarvest is per land-use type, or per raster cell? Likely the former

        bool ISiteVar<bool>.this[Site site]
        {
            get
            {
                return SiteVars.LandUse[site].AllowHarvest;
            }
            set
            {
                //SiteVars.LandUse[site].AllowHarvest = value;
                throw new System.InvalidOperationException("Site variable is read-only");
            }
        }

        bool ISiteVar<bool>.ActiveSiteValues
        {
            set
            {
                throw new System.InvalidOperationException("Site variable is read-only");
            }
        }

        bool ISiteVar<bool>.InactiveSiteValues
        {
            set
            {
                throw new System.InvalidOperationException("Site variable is read-only");
            }
        }

        bool ISiteVar<bool>.SiteValues
        {
            set
            {
                throw new System.InvalidOperationException("Site variable is read-only");
            }
        }
        #endregion
    }
}
