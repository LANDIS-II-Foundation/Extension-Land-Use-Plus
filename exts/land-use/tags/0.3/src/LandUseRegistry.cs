// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/exts/land-use/trunk/

using System.Collections.Generic;

namespace Landis.Extension.LandUse
{
    public static class LandUseRegistry
    {
        private static Dictionary<ushort, LandUse> registry;

        //---------------------------------------------------------------------

        static LandUseRegistry()
        {
            registry = new Dictionary<ushort, LandUse>();
        }

        //---------------------------------------------------------------------

        public static void Register(LandUse landUse)
        {
            if (landUse == null)
                throw new System.ArgumentNullException();
            registry[landUse.MapCode] = landUse;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Look up a land use by its map code.
        /// </summary>
        public static LandUse LookUp(ushort mapCode)
        {
            LandUse landUse;
            if (registry.TryGetValue(mapCode, out landUse))
                return landUse;
            else
                return null;
        }
    }
}
