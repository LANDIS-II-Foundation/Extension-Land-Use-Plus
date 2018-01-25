// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   https://github.com/LANDIS-II-Foundation/Extension-Land-Use-Change

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.LandUse
{
    public class Parameters
    {
        private int timestep;
        private string inputMapNameTemplate;
        private string siteLogPath;

        //Input variables for LANDIS Pause scripting
        private string externalScriptPath;
        private string externalScriptExecutable;
        private string externalScriptCommand;

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

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the CSV log file with biomass harvested per species at
        /// each timestep by individual sites.
        /// </summary>
        public string SiteLogPath
        {
            get
            {
                return siteLogPath;
            }
            set
            {
                if (value == "")
                    throw new InputValueException(value.ToString(),
                                                  "Site log path is empty string");
                siteLogPath = value;
            }
        }

        /// <summary>
        /// Path to the external script used during LANDIS Pause functionality
        /// </summary>
        public string ExternalScript
        {
            get
            {
                return externalScriptPath;
            }
            set
            {
                if (value == "")
                    throw new InputValueException(value.ToString(),
                                                  "Please specify a script for use in Pause functionality");
                externalScriptPath = value;
            }
        }

        /// <summary>
        /// Path to the external script used during LANDIS Pause functionality
        /// </summary>
        public string ExternalExecutable
        {
            get
            {
                return externalScriptExecutable;
            }
            set
            {
                if (value == "")
                    throw new InputValueException(value.ToString(),
                                                  "Please specify an executable for processing scripts in Pause functionality");
                externalScriptExecutable = value;
            }
        }

        /// <summary>
        /// Path to the external script used during LANDIS Pause functionality
        /// </summary>
        public string ExternalCommand
        {
            get
            {
                return externalScriptCommand;
            }
            set
            {
                if (value == "")
                    throw new InputValueException(value.ToString(),
                                                  "Please specify a command-line input for use in Pause functionality");
                externalScriptCommand = value;
            }
        }
    }
}
