// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/exts/land-use/trunk/

using Landis.Core;
using Landis.Library.BiomassCohorts;
using Landis.SpatialModeling;
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
        private static StreamWriter logFile;
        private static IDictionary<ISpecies, int> biomassHarvested;

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
            
            biomassHarvested = new Dictionary<ISpecies, int>(Model.Core.Species.Count);
            ResetSiteTotals();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Prepare the log file for the extension's execution during current
        /// timestep.
        /// </summary>
        public static void TimestepSetUp()
        {
            Cohort.AgeOnlyDeathEvent += CohortDied;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Clean up at the end of the extension's execution during the current
        /// timestep.
        /// </summary>
        public static void TimestepTearDown()
        {
            Cohort.AgeOnlyDeathEvent -= CohortDied;
        }

        //---------------------------------------------------------------------

        public static void CohortDied(object sender,
                                      DeathEventArgs eventArgs)
        {
            ICohort cohort = eventArgs.Cohort;
            RecordHarvest(cohort.Species, cohort.Biomass);
        }

        //---------------------------------------------------------------------

        public static void ResetSiteTotals()
        {
            foreach (ISpecies species in Model.Core.Species)
            {
                biomassHarvested[species] = 0;
            }
        }

        //---------------------------------------------------------------------

        public static void RecordHarvest(ISpecies species,
                                         int      biomass)
        {
            biomassHarvested[species] += biomass;
        }

        //---------------------------------------------------------------------

        public static void WriteTotalsFor(ActiveSite site)
        {
            logFile.Write("{0},{1},{2}", Model.Core.CurrentTime, site.Location.Row, site.Location.Column);
            foreach (ISpecies species in Model.Core.Species)
                logFile.Write(",{0}", biomassHarvested[species]);
            logFile.WriteLine();
            ResetSiteTotals();
        }

        //---------------------------------------------------------------------

        public static void Close()
        {
            logFile.Close();
        }
    }
}
