// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/exts/land-use/trunk/

using Landis.Core;

namespace Landis.Extension.LandUse
{
    internal static class Model
    {
        /// <summary>
        /// The model core instance used across the extension's internal
        /// components.
        /// </summary>
        internal static ICore Core { get; set; }
    }
}
