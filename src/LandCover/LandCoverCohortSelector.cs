using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Landis.Utilities;
using Landis.Core;

namespace Landis.Extension.LandUse.LandCover
{
    public class LandCoverCohortSelector : Landis.Library.BiomassHarvest.PartialCohortSelectors 
    {
        public Percentage percentage;
        public LandCoverCohortSelector(Percentage percent)
        {
            this.percentage = percent;
        }
    }
}
