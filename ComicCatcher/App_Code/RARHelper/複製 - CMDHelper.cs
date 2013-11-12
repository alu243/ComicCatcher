using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Diagnostics;
using System.Threading;
namespace CMDHelper
{
    public static class CMDHelper
    {
        public static void ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                //System.Diagnostics.ProcessStartInfo procStartInfo =
                //    new System.Diagnostics.ProcessStartInfo("cmd.exe", "/c " + (command as string));
                //System.Diagnostics.ProcessStartInfo procStartInfo =
                //    new System.Diagnostics.ProcessStartInfo((command as CommandObj).fileName, (command as CommandObj).arguments);

                //System.Diagnostics.ProcessStartInfo procStartInfo =
                //    new System.Diagnostics.ProcessStartInfo("cmd.exe", "/c \"" + (command as CommandObj).fileName.Trim() + "\" " + (command as CommandObj).arguments);

                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd.exe");

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardInput = true;
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                //procStartInfo.WorkingDirectory = 

                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();

                using (var sw = proc.StandardInput)
                {
                    sw.WriteLine("\"" + (command as CommandObj).fileName.Trim() + "\" " + (command as CommandObj).arguments);
                    Thread.Sleep(200);
                    sw.WriteLine("exit");
                    Thread.Sleep(200);
                }

                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                //MessageBox.Show(command as string);
                //MessageBox.Show(result);
                // Display the command output.
                //Console.WriteLine(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static void ExecuteCommandAsync(CommandObj command)
        {
            try
            {
                //Asynchronously start the Thread to process the Execute command request.
                Thread objThread = new Thread(new ParameterizedThreadStart(ExecuteCommandSync));
                //Make the thread as background thread.
                objThread.IsBackground = true;
                //Set the Priority of the thread.
                objThread.Priority = ThreadPriority.Normal;
                //Start the thread.
                objThread.Start(command);
            }
            catch (ThreadStartException objException)
            {
                throw objException;
            }
            catch (ThreadAbortException objException)
            {
                throw objException;
            }
            catch (Exception objException)
            {
                throw objException;
            }
        }

    }
}
public class CommandObj
{
    public string fileName { get; set; }
    public string arguments { get; set; }
}

