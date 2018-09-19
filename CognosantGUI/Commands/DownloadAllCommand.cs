using CognosantGUI.ViewModels;
using Common;
using System;
using System.Windows;
using System.Windows.Input;

namespace CognosantGUI.Commands
{
    class DownloadAllCommand : ICommand
    {
        private ReportVM reportVM;

        public DownloadAllCommand(ReportVM viewModel)
        {
            reportVM = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            if (reportVM.ReportList.Count >= 1 && reportVM.Status == "Ready.") { return true; }
            else { return false; }
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public async void Execute(object parameter)
        {
            reportVM.Status = "Authenticating...";
            bool authenticated = await IO.HttpConnect(IO.ConstructLogin());

            if (authenticated)
            {
                foreach (Common.Models.Report report in reportVM.ReportList)
                {
                    if (report.Enabled == true)
                    {
                        reportVM.Status = "Generating Report - " + report.Name;
                        string requestUrl = IO.RefactorUrl(report.Url);
                        string resultUrl = await IO.HttpGo(requestUrl);

                        while (resultUrl.Contains("\"m_sStatus\": \"working\""))
                        {
                            reportVM.Status = "Waiting for " + report.Name + " to Complete";
                            var postData = IO.GetPostData(resultUrl);
                            System.Threading.Thread.Sleep(3000);
                            resultUrl = await IO.HttpPost(SettingsVM.Instance.UserSettings.Url + "/ibmcognos/cgi-bin/cognos.cgi", postData);
                        }

                        if (resultUrl.Contains(@"var sURL"))
                        {
                            reportVM.Status = "Downloading " + report.Name;
                            await IO.DownloadFile(IO.ExtractDownloadLink(resultUrl), report.Path);
                        }
                        else if (resultUrl.Contains("\"m_sStatus\": \"prompting\"") || resultUrl.Contains("&quot;m_sStatus&quot;: &quot;prompting&quot;"))
                        {
                            MessageBox.Show( report.Name + " prompted for additional information. Please modify its configuration in Cognos. This report was not downloaded.", "ERROR: Prompt(s) detected", MessageBoxButton.OK);
                        }
                        else
                        {
                            throw new Exception("Unknown Response from Cognos Server.");
                        }
                    }
                }
                reportVM.Status = "Ready.";
                CommandManager.InvalidateRequerySuggested();
            }
            else
            {
                string title = "Login Failure";
                string message = "Unable to login to the Cognos server.  Verify that your Username, Password & Data Source are correct and ensure that Cognos is available";
                MessageBoxButton buttons = MessageBoxButton.OK;
                MessageBox.Show(message, title, buttons);
                reportVM.Status = "Ready.";
            }            
        }
    }
}
