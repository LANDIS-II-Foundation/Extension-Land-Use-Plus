// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/exts/land-use/trunk/

using Landis.Library.BiomassCohorts;
using Landis.Library.BiomassHarvest;
using Landis.SpatialModeling;
using log4net;

namespace Landis.Extension.LandUse.LandCover
{
    /// <summary>
    /// A land cover change that removes trees with partial thinning (i.e.,
    /// a percentage of one or more cohorts are harvested).
    /// </summary>
    public class RemoveTreesWithPartialHarvest
        : BiomassCohortHarvest, IChange
    {
        public const string TypeName = "RemoveTrees";
        private static readonly ILog log = LogManager.GetLogger(typeof(RemoveTreesWithPartialHarvest));
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;

        //---------------------------------------------------------------------

        string IChange.Type
        {
            get { return TypeName; }
        }

        //---------------------------------------------------------------------

        public RemoveTreesWithPartialHarvest(Landis.Library.Harvest.ICohortSelector cohortSelector,
                                             PartialCohortSelectors                 partialCohortSelectors)
            : base(cohortSelector, partialCohortSelectors)
        {
            base.Type = Main.ExtType;
        }

        //---------------------------------------------------------------------

        public void ApplyTo(ActiveSite site)
        {
            if (isDebugEnabled)
            {
                log.DebugFormat("    Applying LCC {0} to site {1}; cohorts are:",
                                GetType().Name,
                                site.Location);
                Debug.WriteSiteCohorts(log, site);
            }
            CurrentSite = site;
            Cut(site);
            if (isDebugEnabled)
            {
                log.DebugFormat("    Cohorts after cutting site {0}:",
                                site.Location);
                Debug.WriteSiteCohorts(log, site);
            }
        }

        //---------------------------------------------------------------------

        protected override void Record(int     reduction,
                                       ICohort cohort)
        {
            if (SiteLog.Enabled)
            {
                SiteLog.RecordHarvest(cohort.Species, reduction);
                if (isDebugEnabled)
                    log.DebugFormat("    {0}, age {1}, biomass {2} : reduction = {3}",
                                    cohort.Species.Name,
                                    cohort.Age,
                                    cohort.Biomass,
                                    reduction);
            }
        }
    }
}
