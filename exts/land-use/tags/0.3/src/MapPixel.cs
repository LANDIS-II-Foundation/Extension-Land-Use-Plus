// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/exts/land-use/trunk/

using Landis.SpatialModeling;

namespace Landis.Extension.LandUse
{
    public class MapPixel : Pixel
    {
        public Band<ushort> LandUseCode  = "Map code for a site's land use";

        public MapPixel()
        {
            SetBands(LandUseCode);
        }
    }
}
