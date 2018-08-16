using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;
using Landis.Library.Succession;
using Landis.SpatialModeling;
using Landis.Extension.Succession.BiomassPnET;
using Landis.Library.SiteHarvest;
using Landis.Library.BiomassHarvest;
using Landis.Library.BiomassCohorts;

using System;
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

        struct CohortData 
        { string SpeciesName; AgeRange AgeRange; public CohortData(string species, AgeRange ageRange) { this.SpeciesName = species; this.AgeRange = ageRange; } }
        private Dictionary<CohortData, bool> speciesCohortsChecked;

        string IChange.Type { get { return TypeName; } }
        bool IChange.Repeat { get { return repeat; } }

        public InsectDefoliation(Dictionary<string,LandCoverCohortSelector> selectors, Planting.SpeciesList speciesToPlant, bool repeatHarvest)
        {
            
            landCoverSelectors = new Dictionary<string, LandCoverCohortSelector>();
            speciesCohortsChecked = new Dictionary<CohortData, bool>();
            foreach (KeyValuePair<string, LandCoverCohortSelector> kvp in selectors)
            {
                //Key: Species name
                //Value: Cohort-based age selector
                foreach(AgeRange ag in kvp.Value.AgeRanges)
                {
                    CohortData cohortData = new CohortData(kvp.Key, ag);
                    speciesCohortsChecked.Add(cohortData, false);
                }
                
                landCoverSelectors[kvp.Key] = kvp.Value;
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

        /// <summary>
        /// Passed anonymously to succession modules to compute defoliation
        /// In succession modules defoliation is computed per-cohort, hence cohortBiomass parameter
        /// </summary>
        /// <param name="active"></param>
        /// <param name="species"></param>
        /// <param name="cohortBiomass"></param>
        /// <param name="siteBiomass"></param>
        /// <returns></returns>
        //public static double InsectDefoliate(ActiveSite active, ISpecies species, int cohortBiomass, int siteBiomass)
        public static double InsectDefoliate(ICohort cohort, ActiveSite active, int siteBiomass)
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
                Landis.Extension.Succession.BiomassPnET.Cohort defolCohort = (cohort as Landis.Extension.Succession.BiomassPnET.Cohort);
                if (id.landCoverSelectors.ContainsKey(defolCohort.Species.Name))
                {
                    Percentage percentage;
                    id.landCoverSelectors[defolCohort.Species.Name].Selects(defolCohort, out percentage);
                    totalDefoliation = percentage.Value;

                    AgeRange ar = id.landCoverSelectors[defolCohort.Species.Name].ContainingRange(defolCohort.Age);
                    CohortData cd = new CohortData(defolCohort.Species.Name, ar);

                    if (totalDefoliation > 1.0)  // Cannot exceed 100% defoliation
                        totalDefoliation = 1.0;

                    //If we move into a new cohort class, defoliation will still happen. After the first round of defoliation for a LandUse, should we just disable everything?
                    //Make a note for Meg on the technicalities.
                    if (!id.repeat)
                    { if (id.CheckSpeciesDefoliated(cd)) { totalDefoliation = 0.0f; } }
                    id.CheckSpecies(cd); //First cohort stops us
                }
            }
            
            return totalDefoliation;
        }

        void ClearSpeciesDefoliated()
        {
            List<CohortData> spp = new List<CohortData>(speciesCohortsChecked.Keys);
            for (int i = 0; i < speciesCohortsChecked.Keys.Count; i++ )
            {
                speciesCohortsChecked[spp[i]] = false;
            }
        }

        bool CheckSpeciesDefoliated(CohortData cd)
        {
            return speciesCohortsChecked[cd];
        }

        private void CheckSpecies(CohortData cd)
        {
            speciesCohortsChecked[cd] = true;
        }

        public void PrintLandCoverDetails()
        {
            Model.Core.UI.WriteLine("Insect defoliation details: ");
            foreach (KeyValuePair<string, LandCoverCohortSelector> kvp in landCoverSelectors)
            {
                //Model.Core.UI.WriteLine("Species: " + kvp.Key + " " + "Rate: " + kvp.Value.percentage.ToString());
            }
        }
    }
}
 