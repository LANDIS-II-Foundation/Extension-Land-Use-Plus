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
        public string ExternalEngine{ get; private set; }
        public string ExternalCommand { get; private set; }
        
        public Pause(string script, string engine, string command)
        {
            ExternalScript = script;
            ExternalEngine = engine;
            ExternalCommand = command;
        }

        public void PauseTimestep()
        {
            Model.Core.UI.WriteLine("Current time: ", Model.Core.CurrentTime);

            //Create an empty lockfile at the appropriate path - may need a separate path for lockfile and rasterfile
            StreamWriter lock_file = new StreamWriter(System.IO.Directory.GetCurrentDirectory() + "/lockfile");
            lock_file.WriteLine(Model.Core.CurrentTime.ToString());
            lock_file.Close();

            Process pause_process;
            if (ExternalCommand != "") //Exhibits preference for custom commands
            {
                pause_process = CallShellScript();
                pause_process.WaitForExit();
                pause_process.Close();
            }
            else if (ExternalEngine != "" && ExternalScript != "")
            {
                pause_process = CallExternalExecutable();
                pause_process.WaitForExit();
                pause_process.Close();
            }
            else
            {
                Model.Core.UI.WriteLine("No pause processes specified, continuing normally");
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
            Process python_process = new Process();

            python_process.StartInfo.FileName = ExternalEngine;
            python_process.StartInfo.UseShellExecute = false;
            python_process.StartInfo.CreateNoWindow = true;
            python_process.StartInfo.Arguments = ExternalScript;
            python_process.StartInfo.RedirectStandardOutput = true;

            Model.Core.UI.WriteLine(python_process.StartInfo.FileName);
            try
            {
                python_process.Start(); // start the process (the python program)
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

            return python_process;
        }

        public void PrintPause()
        {
            Model.Core.UI.WriteLine("Pause routines: ");
            Model.Core.UI.WriteLine("External script path: " + ExternalScript);
            Model.Core.UI.WriteLine("External script executable: " + ExternalEngine);
            Model.Core.UI.WriteLine("External command to execute: " + ExternalCommand);
        }
    }
}
