using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using Microsoft.Win32;

namespace Landis.Extension.LandUse
{
    class Pause
    {
        public string ExternalScript { get; private set; }
        public string ExternalExecutable{ get; private set; }
        public string ExternalCommand { get; private set; }

        public bool UsePause { get; private set; }
        private bool useScript = false;
        private bool useShell = false;
        
        public Pause(string script, string engine, string command)
        {
            ExternalScript = script;
            ExternalExecutable = engine;
            ExternalCommand = command;

            if (ExternalCommand != "" && ExternalCommand != null)
            {
                useShell = true;
                useScript = false;
            }
            else if (ExternalScript != "" && ExternalExecutable != "" && ExternalScript != null && ExternalExecutable != null)
            {
                useScript = true;
                useShell = false;
            }

            if (!useShell && !useScript)
                UsePause = false;
            else
                UsePause = true;
        }

        public void PauseTimestep()
        {
            Model.Core.UI.WriteLine("Current time: ", Model.Core.CurrentTime);

            //Create an empty lockfile at the appropriate path - write model timestep to the contents
            StreamWriter lock_file = new StreamWriter(System.IO.Directory.GetCurrentDirectory() + "/lockfile");
            lock_file.WriteLine(Model.Core.CurrentTime.ToString());
            lock_file.Close();

            Process pause_process = null;
            if (useShell) //Exhibits preference for custom commands
            {
                pause_process = CallShellScript();
                pause_process.WaitForExit();
                pause_process.Close();
            }
            else
            {
                pause_process = CallExternalExecutable();
                pause_process.WaitForExit();
                Model.Core.UI.WriteLine(pause_process.StandardOutput.ReadToEnd());
                pause_process.Close();
            }
        }

        //Using a command shell to evoke arbitrary processes specified by the user
        public Process CallShellScript()
        {
            Model.Core.UI.WriteLine("Starting external shell...");
            Process shell_process = new Process();

            shell_process.StartInfo.UseShellExecute = true;
            shell_process.StartInfo.CreateNoWindow = true;
            shell_process.StartInfo.FileName = "CMD.exe";
            shell_process.StartInfo.Arguments = "/C " + ExternalCommand;
            shell_process.StartInfo.RedirectStandardOutput = false;

            try
            {
                shell_process.Start(); // start the process 
            }
            catch (Win32Exception w)
            {
                Model.Core.UI.WriteLine(w.Message);
                Model.Core.UI.WriteLine(w.ErrorCode.ToString());
                Model.Core.UI.WriteLine(w.NativeErrorCode.ToString());
                Model.Core.UI.WriteLine(w.StackTrace);
                Model.Core.UI.WriteLine(w.Source);
                Exception e = w.GetBaseException();
                Model.Core.UI.WriteLine(e.Message);
            }

            return shell_process;
        }

        //Directly running a script using a scripting engine executable
        public Process CallExternalExecutable()
        {
            Model.Core.UI.WriteLine("Starting external process...");
            Process external_process = new Process();

            external_process.StartInfo.FileName = ExternalExecutable;
            external_process.StartInfo.UseShellExecute = false;
            external_process.StartInfo.CreateNoWindow = true;
            external_process.StartInfo.Arguments = ExternalScript;
            external_process.StartInfo.RedirectStandardOutput = true;

            try
            {
                external_process.Start(); // start the process (the python program)
            }
            catch (Win32Exception w)
            {
                Model.Core.UI.WriteLine(w.Message);
                Model.Core.UI.WriteLine(w.ErrorCode.ToString());
                Model.Core.UI.WriteLine(w.NativeErrorCode.ToString());
                Model.Core.UI.WriteLine(w.StackTrace);
                Model.Core.UI.WriteLine(w.Source);
                Exception e = w.GetBaseException();
                Model.Core.UI.WriteLine(e.Message);
            }

            return external_process;
        }

        public void PrintPause()
        {
            Model.Core.UI.WriteLine("Pause routines: ");
            Model.Core.UI.WriteLine("External script path: " + ExternalScript);
            Model.Core.UI.WriteLine("External script executable: " + ExternalExecutable);
            Model.Core.UI.WriteLine("External command to execute: " + ExternalCommand);
        }
    }
}
