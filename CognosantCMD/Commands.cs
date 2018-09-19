using Common;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CognosantCMD
{
    public class Commands
    {
        public static async Task DownloadAll(ObservableCollection<Common.Models.Report> list)
        {
            Console.WriteLine("Authenticating...");
            bool authenticated = await IO.HttpConnect(IO.ConstructLogin());

            if (authenticated)
            {
                foreach (Common.Models.Report report in list)
                {
                    if (report.Enabled == true)
                    {
                        Console.WriteLine("Generating Report - " + report.Name);
                        string requestUrl = IO.RefactorUrl(report.Url);
                        string resultUrl = await IO.HttpGo(requestUrl);

                        while (resultUrl.Contains("\"m_sStatus\": \"working\""))
                        {
                            Console.WriteLine("Waiting for " + report.Name + " to Complete");
                            var postData = IO.GetPostData(resultUrl);
                            System.Threading.Thread.Sleep(3000);
                            resultUrl = await IO.HttpPost(SettingsVM.Instance.UserSettings.Url + "/ibmcognos/cgi-bin/cognos.cgi", postData);
                        }

                        if (resultUrl.Contains(@"var sURL"))
                        {
                            Console.WriteLine("Downloading " + report.Name);
                            await IO.DownloadFile(IO.ExtractDownloadLink(resultUrl), report.Path);
                        }
                        else if (resultUrl.Contains("\"m_sStatus\": \"prompting\"") || resultUrl.Contains("&quot;m_sStatus&quot;: &quot;prompting&quot;"))
                        {
                            Console.WriteLine(report.Name + " prompted for additional information and was not downloaded.");
                        }
                        else
                        {
                            throw new Exception("Unknown Response from Cognos Server.");
                        }
                    }
                }
                Console.WriteLine("Complete.");
            }
            else
            {
                Console.WriteLine("Unable to login to the Cognos server.  Verify that your Username and Password are correct and ensure that Cognos is available");
            }            
        }

        public static async Task DownloadSingle(string url, string path)
        {
            Console.WriteLine("Authenticating...");
            bool authenticated = await IO.HttpConnect(IO.ConstructLogin());

            if (authenticated)
            {
                Console.WriteLine("Generating Report...");
                string requestUrl = IO.RefactorUrl(url);
                string resultUrl = await IO.HttpGo(requestUrl);

                while (resultUrl.Contains("\"m_sStatus\": \"working\""))
                {
                    Console.WriteLine("Waiting for Report to Complete");
                    var postData = IO.GetPostData(resultUrl);
                    System.Threading.Thread.Sleep(3000);
                    resultUrl = await IO.HttpPost(SettingsVM.Instance.UserSettings.Url + "/ibmcognos/cgi-bin/cognos.cgi", postData);
                }

                if (resultUrl.Contains(@"var sURL"))
                {
                    Console.WriteLine("Downloading...");
                    await IO.DownloadFile(IO.ExtractDownloadLink(resultUrl), path);

                    Console.WriteLine("Complete.");
                }
                else if (resultUrl.Contains("\"m_sStatus\": \"prompting\"") || resultUrl.Contains("&quot;m_sStatus&quot;: &quot;prompting&quot;"))
                {
                    Console.WriteLine("The report prompted for additional information. It was not downloaded.") ;
                }
                else
                {
                    throw new Exception("Unknown Response from Cognos Server.");
                }
            }
            else
            {
                Console.WriteLine("Unable to login to the Cognos server.  Verify that your Username and Password are correct and ensure that Cognos is available");
            }            
        }
    }
}
