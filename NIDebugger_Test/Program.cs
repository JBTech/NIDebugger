﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NonIntrusive;
using System.Threading;
using System.Diagnostics;
namespace NIDebugger_Test
{
    class Program
    {
        static NIDebugger debug = new NIDebugger();
        static uint memoryCave;
        static void Main(string[] args)
        {
            ChangeTitle();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        static void ChangeTitle()
        {
            NIStartupOptions opts = new NonIntrusive.NIStartupOptions();
            opts.executable = @"c:\windows\system32\notepad.exe";
            opts.resumeOnCreate = false;

            // work
            debug.Execute(opts)
                .AllocateMemory(100, out memoryCave)
                .WriteString(memoryCave, "Welcome To NIDebugger", Encoding.Unicode)
                .While(IsStillRunning, OverwriteText)
                .Detach();
        }

        public static bool IsStillRunning()
        {
            return debug.Process.HasExited == false;
        }

        public static void OverwriteText()
        {
            uint oldValue;
            String oldString = "";

            debug.SingleStep()
                .SetProcBP("user32.dll", "SetWindowTextW")
                .Continue()
                .ReadStackValue(8, out oldValue)
                .ReadString(oldValue, 100, Encoding.Unicode, out oldString)
                .WriteStackValue(8, memoryCave);

            Console.WriteLine("Old value: " + oldString);
        }
    }
}
