// This file is part of the Land Use extension for LANDIS-II.
// For licensing information, see the LICENSE
// files in this project's top-level directory, and at:
//   https://github.com/LANDIS-II-Foundation/Extension-Land-Use-Change

using Landis.Utilities;
using Landis.Core;
using Landis.Library.BiomassHarvest;
using Landis.Library.SiteHarvest;
using Landis.Library.Succession;
using System.Collections.Generic;

namespace Landis.Extension.LandUse
{
    /// <summary>
    /// A parser that reads the extension's input and output parameters from
    /// a text file.
    /// </summary>
    public class ParameterParser
        : BasicParameterParser<Parameters>
    {
        // Singleton for all the land uses that have no land cover changes
        private static LandCover.IChange noLandCoverChange = new LandCover.NoChange();

        //---------------------------------------------------------------------

        public override string LandisDataValue
        {
            get {
                return Main.ExtensionName;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public ParameterParser(ISpeciesDataset speciesDataset)
            : base(speciesDataset, false)
            // The "false" above --> keywords are disabled for cohort selectors
        {
        }

        //---------------------------------------------------------------------

        protected override Parameters Parse()
        {
            ReadLandisDataVar();

            Parameters parameters = new Parameters();

            InputVar<int> timestep = new InputVar<int>("Timestep");
            ReadVar(timestep);
            parameters.Timestep = timestep.Value;

            InputVar<string> inputMaps = new InputVar<string>("InputMaps");
            ReadVar(inputMaps);
            parameters.InputMaps = inputMaps.Value;

            InputVar<string> siteLog = new InputVar<string>("SiteLog");
            if (ReadOptionalVar(siteLog))
                parameters.SiteLogPath = siteLog.Value;
            else
                parameters.SiteLogPath = null;

            //Adding parse for script engine location, script name, script command line input
            InputVar<string> pauseScript = new InputVar<string>("ExternalScript");
            if (ReadOptionalVar(pauseScript))
                parameters.ExternalScript = pauseScript.Value;
            else
                parameters.ExternalScript = null;

            InputVar<string> pauseEngine = new InputVar<string>("ExternalExecutable");
            if (ReadOptionalVar(pauseEngine))
                parameters.ExternalExecutable = pauseEngine.Value;
            else
                parameters.ExternalExecutable = null;

            //Adding parse for script engine location, script name, script command line input
            InputVar<string> pauseCommand = new InputVar<string>("ExternalCommand");
            if (ReadOptionalVar(pauseCommand))
                parameters.ExternalCommand = pauseCommand.Value;
            else
                parameters.ExternalCommand = null;

            ReadLandUses();
            return parameters;
        }

        //---------------------------------------------------------------------

        protected void ReadLandUses()
        {
            InputVar<string> name = new InputVar<string>("LandUse");
            InputVar<ushort> mapCode = new InputVar<ushort>("MapCode");
            InputVar<bool> allowHarvest = new InputVar<bool>("AllowHarvest?");
            InputVar<string> landCoverChangeType = new InputVar<string>("LandCoverChange");
            
            Dictionary<string, int> nameLineNumbers = new Dictionary<string, int>();
            Dictionary<ushort, int> mapCodeLineNumbers = new Dictionary<ushort, int>();
            PartialThinning.InitializeClass();
            while (!AtEndOfInput)
            {
                int nameLineNum = LineNumber;
                ReadVar(name);
                int lineNumber;
                if (nameLineNumbers.TryGetValue(name.Value.Actual, out lineNumber))
                    throw new InputValueException(name.Value.String,
                                                  "The land use \"{0}\" was previously used on line {1}",
                                                  name.Value.Actual, lineNumber);
                else
                {
                    nameLineNumbers[name.Value.Actual] = nameLineNum;
                }

                int mapCodeLineNum = LineNumber;
                ReadVar(mapCode);
                if (mapCodeLineNumbers.TryGetValue(mapCode.Value.Actual, out lineNumber))
                    throw new InputValueException(mapCode.Value.String,
                                                  "The map code \"{0}\" was previously used on line {1}",
                                                  mapCode.Value.Actual, lineNumber);
                else
                    mapCodeLineNumbers[mapCode.Value.Actual] = mapCodeLineNum;

                ReadVar(allowHarvest);

                // By default, a land use allows trees to establish.
                bool allowEstablishment = true;
                if (ReadPreventEstablishment())
                    allowEstablishment = false;

                Dictionary<string, LandCover.IChange> landCoverChanges = 
                    new Dictionary<string, LandCover.IChange>();
                List<LandCover.IChange> landCoverList = new List<LandCover.IChange>();
                ReadVar(landCoverChangeType);
                LandCover.IChange landCoverChange = ProcessLandCoverChange(landCoverChangeType);
                landCoverChanges[landCoverChange.Type] = landCoverChange;
                landCoverList.Add(landCoverChange);
                while (ReadOptionalVar(landCoverChangeType)) //Get extra LandCoverChanges
                {
                    if (landCoverChanges.TryGetValue(landCoverChangeType.Value.Actual, out landCoverChange))
                    {
                        throw new InputValueException(landCoverChangeType.Value.String,
                                                  "The land cover change \"{0}\" has already been defined for land use: {1}",
                                                  landCoverChangeType.Value.Actual, name.Value.Actual);
                    }
                    else
                    {
                        landCoverChange = ProcessLandCoverChange(landCoverChangeType);
                        landCoverChanges[landCoverChange.Type] = landCoverChange;
                        landCoverList.Add(landCoverChange);
                    }
                }

                LandCover.IChange[] changes = new LandCover.IChange[landCoverList.Count];
                for (int i = 0; i < landCoverList.Count; i++)
                    changes[i] = landCoverList[i];

                LandUse landUse = new LandUse(name.Value.Actual,
                                              mapCode.Value.Actual,
                                              allowHarvest.Value.Actual,
                                              allowEstablishment,
                                              changes);
                LandUseRegistry.Register(landUse);
            }
        }

        LandCover.IChange ProcessLandCoverChange(InputVar<string> landCoverChangeType)
        {
            InputVar<bool> repeatableHarvest = new InputVar<bool>("RepeatHarvest?");
            bool repeatHarvest = false;
            if (ReadOptionalVar(repeatableHarvest))
            {
                repeatHarvest = repeatableHarvest.Value.Actual;
            }
            LandCover.IChange landCoverChange = null;
            if (landCoverChangeType.Value.Actual == LandCover.NoChange.TypeName)
                landCoverChange = noLandCoverChange;
            else if (landCoverChangeType.Value.Actual == LandCover.RemoveTrees.TypeName)
            {
                LandCover.LandCover.DontParseTrees = true;
                InputValues.Register<AgeRange>(PartialThinning.ReadAgeOrRange);
                ICohortSelector selector = selector = ReadSpeciesAndCohorts("LandUse",
                                                        ParameterNames.Plant,
                                                        "Tony Bonanza",
                                                        "LandCoverChange");
                ICohortCutter cohortCutter = CohortCutterFactory.CreateCutter(selector,
                                                                              Main.ExtType);
                Planting.SpeciesList speciesToPlant = ReadSpeciesToPlant();
                landCoverChange = new LandCover.RemoveTrees(cohortCutter, speciesToPlant, repeatHarvest);
                LandCover.LandCover.DontParseTrees = false;
            }
            else if (landCoverChangeType.Value.Actual == LandCover.InsectDefoliation.TypeName)
            {
                //Insects will reduce biomass of cohorts rather than directly affecting demographics       
                InputValues.Register<AgeRange>(LandCover.LandCover.ReadAgeOrRange);
                ICohortSelector selector = ReadSpeciesAndCohorts("LandUse",
                                                            ParameterNames.Plant,
                                                            "Vito Tortellini",
                                                            "LandCoverChange");
                Planting.SpeciesList speciesToPlant = ReadSpeciesToPlant();
                landCoverChange = new LandCover.InsectDefoliation(LandCover.LandCover.CohortSelectors, speciesToPlant, repeatHarvest);
            }
            else
                throw new InputValueException(landCoverChangeType.Value.String,
                                              "\"{0}\" is not a type of land cover change",
                                              landCoverChangeType.Value.Actual);
            //landCoverChange.PrintLandCoverDetails();
            return landCoverChange;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a cohort selection method for a specific set of ages and
        /// age ranges.
        /// </summary>
        /// <remarks>
        /// This overrides the base method so it can use the PartialThinning
        /// class to handle cohort selections with percentages. Added support 
        /// for InsectDefoliation via LandCover cohort selections
        /// </remarks>
        protected override void CreateCohortSelectionMethodFor(ISpecies species,
                                                               IList<ushort> ages,
                                                               IList<AgeRange> ranges)
        {
            if (LandCover.LandCover.DontParseTrees)
            {
                if (!PartialThinning.CreateCohortSelectorFor(species, ages, ranges))
                {
                    base.CreateCohortSelectionMethodFor(species, ages, ranges);
                }
            }
            else if (!LandCover.LandCover.CreateCohortSelectorFor(species, ages, ranges))
            {
                base.CreateCohortSelectionMethodFor(species, ages, ranges);
            }
        }
    }
}