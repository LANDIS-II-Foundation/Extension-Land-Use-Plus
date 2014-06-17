// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/exts/land-use/trunk/

using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;
using System.Collections.Generic;

namespace Landis.Extension.LandUse
{
    /// <summary>
    /// A parser that reads the extension's input and output parameters from
    /// a text file.
    /// </summary>
    public class ParameterParser
        : TextParser<Parameters>
    {
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
        public ParameterParser()
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

            ReadLandUses();
            return parameters;
        }

        //---------------------------------------------------------------------

        protected void ReadLandUses()
        {
            InputVar<string> name = new InputVar<string>("LandUse");
            InputVar<ushort> mapCode = new InputVar<ushort>("MapCode");
            InputVar<bool> allowHarvest = new InputVar<bool>("AllowHarvest?");

            Dictionary<string, int> nameLineNumbers = new Dictionary<string, int>();
            Dictionary<ushort, int> mapCodeLineNumbers = new Dictionary<ushort, int>();

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

                LandUse landUse = new LandUse(name.Value.Actual,
                                              mapCode.Value.Actual,
                                              allowHarvest.Value.Actual,
                                              null /* Land Cover Change */);
                LandUseRegistry.Register(landUse);
            }
        }
    }
}