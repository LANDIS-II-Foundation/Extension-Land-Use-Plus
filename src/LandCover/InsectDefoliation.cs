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

        string IChange.Type { get { return TypeName; } }
        bool IChange.Repeat { get { return repeat; } }

        public InsectDefoliation(Dictionary<string,LandCoverCohortSelector> selectors, Planting.SpeciesList speciesToPlant, bool repeatHarvest)
        {
            landCoverSelectors = new Dictionary<string, LandCoverCohortSelector>();
            foreach (KeyValuePair<string, LandCoverCohortSelector> kvp in selectors)
            {              
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
        public void ApplyTo(ActiveSite site, bool newLandUse)
        {
            if (newLandUse)
            {
                CohortDefoliation.Compute = InsectDefoliate;
            }
            else
            {
                if (!repeat)
                {
                    Model.Core.UI.WriteLine("Disable Insects");
                    CohortDefoliation.Compute = DontCompute;
                }
            }
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
                    Model.Core.UI.WriteLine(defolCohort.Species.Name);
                    Model.Core.UI.WriteLine(defolCohort.Age.ToString());
                    Percentage percentage = null;
                    id.landCoverSelectors[defolCohort.Species.Name].Selects(defolCohort, out percentage);

                    if (percentage == null)
                        Model.Core.UI.WriteLine("Null percent");
                    else
                    {
                        Model.Core.UI.WriteLine("Defoliating: " + percentage.ToString());
                        totalDefoliation = percentage.Value;
                    }
                    
                    if (totalDefoliation > 1.0)  // Cannot exceed 100% defoliation
                        totalDefoliation = 1.0;
                }
            }
            
            return totalDefoliation;
        }

        public static double DontCompute(ICohort cohort, ActiveSite active, int siteBiomass)
        {
            return 0;
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
 