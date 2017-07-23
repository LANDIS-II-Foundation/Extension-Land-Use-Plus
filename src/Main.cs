// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   https://github.com/LANDIS-II-Foundation/Extension-Land-Use-Change
//

using Landis.Core;
using Landis.Library.Succession;
using Landis.SpatialModeling;
using log4net;
using System.Collections.Generic;

namespace Landis.Extension.LandUse
{
    public class Main
        : Landis.Core.ExtensionMain
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Main));
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;

        public static readonly ExtensionType ExtType = new ExtensionType("disturbance:land use");
        public static readonly string ExtensionName = "Land Use";

        private Parameters parameters;
        private Pause pauseFunction;
        private string inputMapTemplate;

        //---------------------------------------------------------------------

        public Main()
            : base(ExtensionName, ExtType)
        {
        }

        //---------------------------------------------------------------------

        public override void LoadParameters(string dataFile,
                                            ICore modelCore)
        {
            Model.Core = modelCore;
            Landis.Library.BiomassHarvest.Main.InitializeLib(Model.Core);
            Model.Core.UI.WriteLine("  Loading parameters from {0}", dataFile);
            ParameterParser parser = new ParameterParser(Model.Core.Species);
            parameters = Landis.Data.Load<Parameters>(dataFile, parser);
        }

        //---------------------------------------------------------------------

        public override void Initialize()
        {
            Model.Core.UI.WriteLine("Initializing {0}...", Name);

            SiteVars.Initialize(Model.Core);
            Timestep = parameters.Timestep;
            inputMapTemplate = parameters.InputMaps;

            pauseFunction = new Pause(parameters.ExternalScript, parameters.ExternalExecutable, parameters.ExternalCommand);
            if (!pauseFunction.UsePause)
            {
                Model.Core.UI.WriteLine("No pause processes specified, continuing normally");
                pauseFunction = null;
            }

            if (parameters.SiteLogPath != null)
                SiteLog.Initialize(parameters.SiteLogPath);

            // Load initial land uses from input map for timestep 0
            ProcessInputMap(
                delegate(Site site,
                         LandUse initialLandUse)
                {
                    SiteVars.LandUse[site] = initialLandUse;
                    return initialLandUse.Name;
                });
        }

        //---------------------------------------------------------------------

        public override void Run()
        {
            if (SiteLog.Enabled)
                SiteLog.TimestepSetUp();

            if (pauseFunction != null)
            {
                pauseFunction.PauseTimestep();
            }

            ProcessInputMap(
                delegate(Site site,
                         LandUse newLandUse)
                {
                    LandUse currentLandUse = SiteVars.LandUse[site];
                    string siteKey = null;
                    if (newLandUse != currentLandUse)
                    {
                        SiteVars.LandUse[site] = newLandUse;
                        siteKey = string.Format("{0} --> {1}", currentLandUse.Name, newLandUse.Name);
                        if (!currentLandUse.AllowEstablishment && newLandUse.AllowEstablishment)
                        {
                            Reproduction.EnableEstablishment((ActiveSite)site);
                        }
                        else if (currentLandUse.AllowEstablishment && !newLandUse.AllowEstablishment)
                        {
                            Reproduction.PreventEstablishment((ActiveSite)site);
                        }

                        if (isDebugEnabled)
                            log.DebugFormat("    LU at {0}: {1}", site.Location, siteKey);

                        for (int i = 0; i < newLandUse.LandCoverChanges.Length; i++)
                        {
                            LandCover.IChange LandCoverChange = newLandUse.LandCoverChanges[i];
                            LandCoverChange.ApplyTo((ActiveSite)site);
                        }
                    }
                    else
                    {                        
                        for (int i = 0; i < currentLandUse.LandCoverChanges.Length; i++)
                        {
                            LandCover.IChange LandCoverChange = newLandUse.LandCoverChanges[i];
                            if(LandCoverChange.Repeat)
                                LandCoverChange.ApplyTo((ActiveSite)site);
                        }
                        //Do we need to set the siteKey to mess with anything if we repeat harvests?
                        //siteKey = string.Format("{0} --> {1}", currentLandUse.Name, currentLandUse.Name);
                    }

                    if (SiteLog.Enabled)
                        SiteLog.WriteTotalsFor((ActiveSite)site);

                    return siteKey;
                });

            if (SiteLog.Enabled)
                SiteLog.TimestepTearDown();
        }

        //---------------------------------------------------------------------

        // A delegate for processing a land use read from an input map.
        public delegate string ProcessLandUseAt(Site site, LandUse landUse);

        //---------------------------------------------------------------------
        
        //Modified to add inputMapPath, allowing users to specify raster paths to change at timestep
        public void ProcessInputMap(ProcessLandUseAt processLandUseAt)
        {
            string inputMapPath = MapNames.ReplaceTemplateVars(inputMapTemplate, Model.Core.CurrentTime);
            Model.Core.UI.WriteLine("  Reading map \"{0}\"...", inputMapPath);

            IInputRaster<MapPixel> inputMap;
            Dictionary<string, int> counts = new Dictionary<string, int>();

            using (inputMap = Model.Core.OpenRaster<MapPixel>(inputMapPath))
            {
                MapPixel pixel = inputMap.BufferPixel;
                foreach (Site site in Model.Core.Landscape.AllSites)
                {
                    inputMap.ReadBufferPixel();
                    if (site.IsActive)
                    {
                        LandUse landUse = LandUseRegistry.LookUp(pixel.LandUseCode.Value);
                        if (landUse == null)
                        {
                            string message = string.Format("Error: Unknown map code ({0}) at pixel {1}",
                                                           pixel.LandUseCode.Value,
                                                           site.Location);
                            throw new System.ApplicationException(message);
                        }
                        string key = processLandUseAt(site, landUse);
                        if (key != null)
                        {
                            int count;
                            if (counts.TryGetValue(key, out count))
                                count = count + 1;
                            else
                                count = 1;
                            counts[key] = count;
                        }
                    }
                }
            }
            foreach (string key in counts.Keys)
                Model.Core.UI.WriteLine("    {0} ({1:#,##0})", key, counts[key]);
        }    

        //---------------------------------------------------------------------

        public new void CleanUp()
        {
            if (SiteLog.Enabled)
                SiteLog.Close();
        }
    }

    //----------------------------------------------------
}
