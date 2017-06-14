using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Core;
using Landis.SpatialModeling;
using Landis.Extension.Insects;

namespace Landis.Extension.LandUse
{
     /**
      * A class demonstrating LU+ connectivity to succession modules
      * and Insect extension. Can directly call static Defoliation 
      * methods from Insect library
      **/
    class InsectDefoliant
    {
        public InsectDefoliant()
        {

        }

        public void InsectDefoliate(ActiveSite active, ISpecies species, int cohortBiomass, int siteBiomass)
        {
            //Do LU+ specific things?
            Defoliate.DefoliateCohort(active, species, cohortBiomass, siteBiomass);
        }
    }
}
