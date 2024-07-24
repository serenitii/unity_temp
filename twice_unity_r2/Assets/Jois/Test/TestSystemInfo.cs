using System.Collections;
using System.Collections.Generic;

using System;
using System.Diagnostics;

using UnityEngine;
using Debug = UnityEngine.Debug;


public class TestSystemInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Define variables to track the peak
        // memory usage of the process.
        long peakPagedMem = 0,
            peakWorkingSet = 0,
            peakVirtualMem = 0;

        // Start the process.
        using (Process myProcess = Process.GetCurrentProcess()) //.Start("NotePad.exe"))
        {
            // Display the process statistics until
            // the user closes the program.
            //do
            {
                if (!myProcess.HasExited)
                {
                    // Refresh the current process property values.
                    myProcess.Refresh();


                    // Display current process statistics.

                    UnityEngine.Debug.Log($"{myProcess} -");
                    UnityEngine.Debug.Log("-------------------------------------");

                    UnityEngine.Debug.Log($"  Physical memory usage     : {myProcess.WorkingSet64}");
                    UnityEngine.Debug.Log($"  Base priority             : {myProcess.BasePriority}");
                    UnityEngine.Debug.Log($"  Priority class            : {myProcess.PriorityClass}");
                    UnityEngine.Debug.Log($"  User processor time       : {myProcess.UserProcessorTime}");
                    UnityEngine.Debug.Log($"  Privileged processor time : {myProcess.PrivilegedProcessorTime}");
                    UnityEngine.Debug.Log($"  Total processor time      : {myProcess.TotalProcessorTime}");
                    UnityEngine.Debug.Log($"  Paged system memory size  : {myProcess.PagedSystemMemorySize64}");
                    UnityEngine.Debug.Log($"  Paged memory size         : {myProcess.PagedMemorySize64}");

                    // Update the values for the overall peak memory statistics.
                    peakPagedMem = myProcess.PeakPagedMemorySize64;
                    peakVirtualMem = myProcess.PeakVirtualMemorySize64;
                    peakWorkingSet = myProcess.PeakWorkingSet64;

                    if (myProcess.Responding)
                    {
                        UnityEngine.Debug.Log("Status = Running");
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Status = Not Responding");
                    }
                }
            } //while (!myProcess.WaitForExit(1000));

            Debug.Log("\n");
            UnityEngine.Debug.Log($"  Process exit code          : {myProcess.ExitCode}");

            // Display peak memory statistics for the process.
            UnityEngine.Debug.Log($"  Peak physical memory usage : {peakWorkingSet}");
            UnityEngine.Debug.Log($"  Peak paged memory usage    : {peakPagedMem}");
            UnityEngine.Debug.Log($"  Peak virtual memory usage  : {peakVirtualMem}");
        }
    }
}