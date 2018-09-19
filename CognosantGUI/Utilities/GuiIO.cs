using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace CognosantGUI.Utilities
{
    class GuiIO
    {
        public static bool FileNameValid(string value)
        {
            return Path.GetInvalidFileNameChars().All(invalidChar => value.Contains(invalidChar) == false);
        }

        public static string GetReportFormat(string link)
        {
            string reportFormat = ".txt";
            string decodedUrl = Uri.UnescapeDataString(link);
            string[] parameters = decodedUrl.Split('&');

            foreach (string parameter in parameters)
            {
                if (parameter.Length > 17 && parameter.Substring(0, 17) == "run.outputFormat=")
                {
                    reportFormat = "." + parameter.Remove(0, 17).ToLower();
                }
                else if (parameter.Length > 10 && parameter.Substring(0, 10) == "ui.format=")
                {
                    reportFormat = "." + parameter.Remove(0, 10).ToLower();
                }
            }
            return reportFormat;
        }

        public static string GetReportName(string link)
        {
            string reportName = "[ERROR: Name Not Found]";
            string decodedUrl = Uri.UnescapeDataString(link);
            string[] parameters = decodedUrl.Split('&');

            foreach (string parameter in parameters)
            {
                if (parameter.Substring(0, 8) == "ui.name=")
                {
                    reportName = parameter.Remove(0, 8);
                }
            }
            return reportName;
        }

        public static string VerifyDownloadPath(string value)
        {
            if (Path.HasExtension(value))
            {
                if (Directory.Exists(Path.GetDirectoryName(value)))
                {
                    return value;
                }
                else
                {
                    throw new Exception("Folder does not exist.");
                }
            }
            else
            {
                if (value.EndsWith("."))
                {
                    if (Directory.Exists(Path.GetDirectoryName(value)))
                    {
                        return value;
                    }
                    else
                    {
                        throw new Exception("Folder does not exist.");
                    }
                }
                else
                {
                    if (Directory.Exists(value))
                    {
                        return value.Replace(@"\\", @"\") + @"\download.txt";
                    }
                    else
                    {
                        throw new Exception("Folder does not exist.");
                    }
                }
            }
        }
    }
}
