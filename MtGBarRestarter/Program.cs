using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MtGBarRepairbot
{
    class Program
    {
        public static void Main(string[] args)
        {
            // LOL
            Thread.Sleep(4000);

            foreach (string directory in args) {
                if (Directory.Exists(directory)) {
                    try {
                        Directory.Delete(directory, true);
                    }
                    catch (Exception ex) {
                        Console.WriteLine("Exception: " + ex.Message);
                    }
                }
            }

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo("MtGBar.exe");
            p.Start();
        }
    }
}