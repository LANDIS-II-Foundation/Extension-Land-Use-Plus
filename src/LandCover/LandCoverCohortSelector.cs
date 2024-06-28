using System.Collections.Generic;
using Landis.Utilities;
using Landis.Core;
using Landis.Library.SiteHarvest;
using Landis.Library.UniversalCohorts;

namespace Landis.Extension.LandUse.LandCover
{
    public class LandCoverCohortSelector : Landis.Library.BiomassHarvest.SpecificAgesCohortSelector 
    {
        private AgesAndRanges agesAndRanges;
        public List<AgeRange> AgeRanges;
        private IDictionary<ushort, Percentage> percentages;
        public LandCoverCohortSelector(IList<ushort> ages, IList<AgeRange> ranges, IDictionary<ushort, Percentage> percentages)
            : base(ages, ranges, percentages)
        {
            agesAndRanges = new AgesAndRanges(ages, ranges);
            this.percentages = new Dictionary<ushort, Percentage>(percentages);
            AgeRanges = new List<AgeRange>(ranges);
        }

        public AgeRange ContainingRange(ushort age)
        {
            AgeRange? ageRange;
            agesAndRanges.Contains(age, out ageRange);
            return (AgeRange)ageRange;
        }
    }
}
