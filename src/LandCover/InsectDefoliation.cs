using Landis.Core;
using Landis.Utilities;
using Landis.Library.Succession;
using Landis.SpatialModeling;
using Landis.Library.SiteHarvest;
using Landis.Library.BiomassHarvest;
using Landis.Library.BiomassCohorts;
using System.Collections.Generic;
using Landis.Extension.Succession.BiomassPnET;

namespace Landis.Extension.LandUse.LandCover
{
    class InsectDefoliation 
        : IChange
    {
        public const string TypeName = "InsectDefoliation";
        private bool repeat;
        private int harvestTime;
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
                if (!repeat)
                {
                    harvestTime = Model.Core.CurrentTime;
                    //Model.Core.UI.WriteLine("Setting defoliation harvest time: " + harvestTime);
                }
            }
            else
            {
                if (!repeat)    //When repeat harvest is off, shut down the delegate if the land use doesn't transition
                {
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
                    else if (Model.Core.CurrentTime > id.harvestTime)
                    {
                        //Model.Core.UI.WriteLine("Disabling defoliation after harvest time: " + Model.Core.CurrentTime);
                        CohortDefoliation.Compute = DontCompute;
                        id = null;
                        break;
                    }
                    break;
                }
            }

            if (id != null)
            {
                Landis.Extension.Succession.BiomassPnET.Cohort defolCohort = (cohort as Landis.Extension.Succession.BiomassPnET.Cohort);
                if (id.landCoverSelectors.ContainsKey(defolCohort.Species.Name))
                {
                    Percentage percentage = null;
                    id.landCoverSelectors[defolCohort.Species.Name].Selects(defolCohort, out percentage);

                    if (percentage == null)
                        Model.Core.UI.WriteLine("Null percent");
                    else
                    {
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
                Model.Core.UI.WriteLine("Species: " + kvp.Key + " " + "Rate: " + kvp.Value.percentage.ToString());
            }
        }
    }
}
 