// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   https://github.com/LANDIS-II-Foundation/Extension-Land-Use-Change

using Landis.SpatialModeling;

namespace Landis.Extension.LandUse
{
    /// <summary>
    /// The definition of a particular land use.
    /// </summary>
    public class LandUse
    {
        public string Name { get; protected set; }
        public ushort MapCode { get; protected set; }
        public bool AllowHarvest { get; protected set; }
        public bool RepeatHarvest { get; protected set; }
        public bool AllowEstablishment { get; protected set; }
        //May need to generalize to array of LandCoverChanges?
        public LandCover.IChange LandCoverChange { get; protected set; }

        //---------------------------------------------------------------------

        public LandUse(
            string name,
            ushort mapCode,
            bool harvestingAllowed,
            bool establishmentAllowed,
            LandCover.IChange initialLCC)
        {
            Name = name;
            MapCode = mapCode;
            AllowHarvest = harvestingAllowed;
            RepeatHarvest = false;
            AllowEstablishment = establishmentAllowed;
            LandCoverChange = initialLCC;
        }

        public LandUse(
            string name,
            ushort mapCode,
            bool harvestingAllowed,
            bool repeatHarvesting,
            bool establishmentAllowed,
            LandCover.IChange initialLCC)
        {
            Name = name;
            MapCode = mapCode;
            AllowHarvest = harvestingAllowed;
            RepeatHarvest = repeatHarvesting;
            AllowEstablishment = establishmentAllowed;
            LandCoverChange = initialLCC;
        }
    }
}
