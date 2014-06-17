// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/exts/land-use/trunk/

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.LandUse
{
    public class Parameters
    {
        private int timestep;
        private string inputMapNameTemplate;

        //---------------------------------------------------------------------

        public int Timestep
        {
            get
            {
                return timestep;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                                                  "Timestep must be > or = 0");
                timestep = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Template for pathnames for input maps of land use.
        /// </summary>
        public string InputMaps
        {
            get
            {
                return inputMapNameTemplate;
            }
            set
            {
                if (value != null)
                {
                    MapNames.CheckTemplateVars(value);
                }
                inputMapNameTemplate = value;
            }
        }
    }
}
