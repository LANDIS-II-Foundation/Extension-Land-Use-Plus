using Landis.Core;
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
        private Planting.SpeciesList speciesToPlant;
        private Dictionary<string, LandCoverCohortSelector>  landCoverSelectors;
        private Dictionary<string, bool> speciesChecked;

        string IChange.Type { get { return TypeName; } }
        bool IChange.Repeat { get { return repeat; } }

        public InsectDefoliation(Dictionary<string,LandCoverCohortSelector> selectors, Planting.SpeciesList speciesToPlant, bool repeatHarvest)
        {
            landCoverSelectors = new Dictionary<string, LandCoverCohortSelector>();
            speciesChecked = new Dictionary<string, bool>();
            foreach (KeyValuePair<string, LandCoverCohortSelector> kvp in selectors)
            {
                landCoverSelectors[kvp.Key] = kvp.Value;
                speciesChecked.Add(kvp.Key, false);
            }
            CohortDefoliation.Compute = InsectDefoliate;
            this.repeat = repeatHarvest;
            this.speciesToPlant = speciesToPlant;
        }

        /// <summary>
        ///Used to change the intensity of defoliation parameters across Landis.
        /// </summary>
        /// <param name="site"></param>
        public void ApplyTo(ActiveSite site)
        {
            ClearSpeciesDefoliated();
            if (speciesToPlant != null)
                Reproduction.ScheduleForPlanting(speciesToPlant, site);
        }

        //Issues:
        //LandCoverSelectors can't be static
        //Spillover between years (one year after de-activating non-repeats, one year before re-activating repeats)

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
            InsectDefoliation id = null;
            foreach (IChange lcc in SiteVars.LandUse[active].LandCoverChanges)
            {
                if (lcc.GetType() == typeof(InsectDefoliation))
                {
                    id = (lcc as InsectDefoliation);
                    if (id.repeat)
                    {
                        CohortDefoliation.Compute = InsectDefoliate;
                    }
                    break;
                }
            }
            if (id != null)
            {
                if (id.landCoverSelectors.ContainsKey(species.Name))
                {
                    totalDefoliation = id.landCoverSelectors[species.Name].percentage.Value;
                    if (totalDefoliation > 1.0)  // Cannot exceed 100% defoliation
                        totalDefoliation = 1.0;

                    if (!id.repeat)
                    { if (id.CheckSpeciesDefoliated(species.Name)) { totalDefoliation = 0.0f; } }
                    id.CheckSpecies(species.Name);
                }
            }
            else
                Model.Core.UI.WriteLine("Insect defoliation null, something wrong");
            
            return totalDefoliation;
        }

        void ClearSpeciesDefoliated()
        {
            List<string> spp = new List<string>(speciesChecked.Keys);
            for (int i = 0; i < speciesChecked.Keys.Count; i++ )
            {
                speciesChecked[spp[i]] = false;
            }
        }

        bool CheckSpeciesDefoliated(string name)
        {
            return speciesChecked[name];
        }

        public void CheckSpecies(string name)
        {
            speciesChecked[name] = true;
        }

        public void PrintLandCoverDetails()
        {
            Model.Core.UI.WriteLine("Insect defoliation details: ");
            foreach (KeyValuePair<string, LandCoverCohortSelector> kvp in landCoverSelectors)
            {
                Model.Core.UI.WriteLine("Species: " + kvp.Key + " " + "Rate: " + kvp.Value.percentage.ToString());
            }
        }
    }
}
 