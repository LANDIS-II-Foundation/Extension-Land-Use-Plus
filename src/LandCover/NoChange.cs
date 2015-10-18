// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   https://github.com/LANDIS-II-Foundation/Extension-Land-Use-Change

using Landis.SpatialModeling;

namespace Landis.Extension.LandUse.LandCover
{
    /// <summary>
    /// For land uses that do not trigger any change in the land cover.
    /// </summary>
    public class NoChange
        : IChange
    {
        public const string TypeName = "NoChange";

        public string Type
        {
            get { return TypeName; }
        }

        public void ApplyTo(ActiveSite site)
        {
            // Do nothing to the site.
        }
    }
}
