// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   https://github.com/LANDIS-II-Foundation/Extension-Land-Use-Change

using Landis.Core;
using Landis.Library.BiomassHarvest;
using Landis.SpatialModeling;
using log4net;
using System.Collections.Generic;
using System.IO;

namespace Landis.Extension.LandUse
{
    /// <summary>
    /// A log file with details about the biomass removed at each site.
    /// </summary>
    public static class SiteLog
    {
        public static bool Enabled { get; private set; }
        private static StreamWriter logFile = null;
        private static readonly ILog log = LogManager.GetLogger(typeof(SiteLog));
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;

        //---------------------------------------------------------------------

        static SiteLog()
        {
            Enabled = false;
        }

        //---------------------------------------------------------------------

        public static void Initialize(string path)
        {
            Model.Core.UI.WriteLine("  Opening log file \"{0}\"...", path);
            logFile = Landis.Data.CreateTextFile(path);
            logFile.Write("timestep,row,column");
            foreach (ISpecies species in Model.Core.Species)
                logFile.Write(",{0}", species.Name);
            logFile.WriteLine();
            Enabled = true;
            SiteBiomass.ResetHarvestTotals();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Prepare the log file for the extension's execution during current
        /// timestep.
        /// </summary>
        public static void TimestepSetUp()
        {
            SiteBiomass.EnableRecordingForHarvest();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Clean up at the end of the extension's execution during the current
        /// timestep.
        /// </summary>
        public static void TimestepTearDown()
        {
            SiteBiomass.DisableRecordingForHarvest();
        }

        //---------------------------------------------------------------------

        public static void WriteTotalsFor(ActiveSite site)
        {
            logFile.Write("{0},{1},{2}", Model.Core.CurrentTime, site.Location.Row, site.Location.Column);
            foreach (ISpecies species in Model.Core.Species)
                logFile.Write(",{0}", SiteBiomass.Harvested[species]);
            logFile.WriteLine();
            SiteBiomass.ResetHarvestTotals();
        }

        //---------------------------------------------------------------------

        public static void Close()
        {
            logFile.Close();
        }
    }
}
