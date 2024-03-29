﻿using System.Text;

namespace ComicCatcherLib.Utils;

public static class CMDUtil
{
    private static void ExecuteCommandSync(object command)
    {
        try
        {
            // create the ProcessStartInfo using "cmd" as the program to be run,
            // and "/c " as the parameters.
            // Incidentally, /c tells cmd that we want it to execute the command that follows,
            // and then exit.
            //System.Diagnostics.ProcessStartInfo procStartInfo =
            //    new System.Diagnostics.ProcessStartInfo("cmd.exe", "/c " + (command as string));
            //string arguments = Encoding.Default.GetString(Encoding.GetEncoding(65000).GetBytes((command as CommandObj).arguments));
            string arguments = Encoding.Default.GetString(Encoding.UTF8.GetBytes((command as CommandObj).arguments));
            System.Diagnostics.ProcessStartInfo procStartInfo =
                new System.Diagnostics.ProcessStartInfo((command as CommandObj).fileName, (command as CommandObj).arguments);


            // The following commands are needed to redirect the standard output.
            // This means that it will be redirected to the Process.StandardOutput StreamReader.
            //procStartInfo.RedirectStandardInput = true;
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            // Do not create the black window.
            procStartInfo.CreateNoWindow = true;

            if (false == string.IsNullOrEmpty((command as CommandObj).workdir))
                procStartInfo.WorkingDirectory = (command as CommandObj).workdir;

            // Now we create a process, assign its ProcessStartInfo and start it
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();

            // Get the output into a string
            string result = proc.StandardOutput.ReadToEnd();
            //MessageBox.Show(command as string);
            //MessageBox.Show(result);
            // Display the command output.
            //Console.WriteLine(result);
        }
        catch (Exception e)
        {
            //NLogger.Error(e.ToString());
            //throw e;
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