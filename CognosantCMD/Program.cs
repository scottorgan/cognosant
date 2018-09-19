using Common.ViewModels;
using System;
using Common;

namespace CognosantCMD
{
    class Program
    {
        static void Main(string[] args)
        {
            var viewModel = new ReportBaseVM();

            if (IO.VerifyUserSettings() == false)
            {
                Console.WriteLine("Cognosant is not fully configured.  Please run CognosantGUI and review the required settings.");
                Environment.Exit(0);
            }

            if (args.Length == 1)
            {
                string argument = args[0];
                if (argument.ToLower() == "/all")
                {
                    Commands.DownloadAll(viewModel.ReportList).Wait();
                }
                else
                {
                    bool validReport = false;
                    foreach (Common.Models.Report report in viewModel.ReportList)
                    {
                        if (report.Name.ToLower() == argument.ToLower())
                        {
                            validReport = true;
                            Commands.DownloadSingle(report.Url, report.Path).Wait();
                        }
                    }

                    if (validReport == false) { Console.WriteLine("Error: Could not find \"" + argument + "\" in the saved report list."); }
                }
            }
            else
            {
                Console.WriteLine("USAGE: Cognosant.exe [Parameter or Report Name] \n  PARAMETERS: \n   /All - Download all reports.");
            }
        }
    }
}
