﻿using Landis.Core;
using Landis.Library.Succession;
using Landis.SpatialModeling;
using Landis.Library.Biomass;
using Landis.Library.SiteHarvest;
using Landis.Library.BiomassHarvest;
using System.Collections.Generic;

namespace Landis.Extension.LandUse.LandCover
{
    class InsectDefoliation 
        : IChange
    {
        public const string TypeName = "InsectDefoliation";
        private bool repeat;
        private static Dictionary<string, LandCoverCohortSelector>  landCoverSelectors;

        string IChange.Type { get { return TypeName; } }
        bool IChange.Repeat { get { return repeat; } }

        public InsectDefoliation(Dictionary<string,LandCoverCohortSelector> selectors, bool repeatHarvest)
        {
            landCoverSelectors = new Dictionary<string, LandCoverCohortSelector>();
            foreach (KeyValuePair<string, LandCoverCohortSelector> kvp in selectors)
            {
                landCoverSelectors[kvp.Key] = kvp.Value;
            }
            CohortDefoliation.Compute = InsectDefoliate;
            this.repeat = repeatHarvest;
        }

        /// <summary>
        ///Used to change the intensity of defoliation parameters across timesteps.
        /// </summary>
        /// <param name="site"></param>
        public void ApplyTo(ActiveSite site)
        {
            //To change percentage values, need a dictionary of species and percentage changes
            //InsectDefoliation.landCoverSelectors[species].percentages[ages].Value = ???
        }

        /// <summary>
        /// Passed anonymously to succession modules to compute defoliation
        /// In succession modules defoliation is computed per-cohort, hence cohortBiomass parameter
        /// </summary>
        /// <param name="active"></param>
        /// <param name="species"></param>
        /// <param name="cohortBiomass"></param>
        /// <param name="siteBiomass"></param>
        /// <returns></returns>
        public static double InsectDefoliate(ActiveSite active, ISpecies species, int cohortBiomass, int siteBiomass)
        {
            double totalDefoliation = 0.0;
            if (landCoverSelectors.ContainsKey(species.Name))
            {
                totalDefoliation = landCoverSelectors[species.Name].percentage.Value;

                if (totalDefoliation > 1.0)  // Cannot exceed 100% defoliation
                    totalDefoliation = 1.0;
            }

            return totalDefoliation;
        }
    }
}
 