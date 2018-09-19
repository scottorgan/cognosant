using CognosantGUI.Commands;
using Common.Models;
using Common.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace CognosantGUI.ViewModels
{
    public class ReportVM : ReportBaseVM
    {
        private string payload, status;
        public bool DebugMode { get; set; }
        public Func<string, string, bool> ConfirmationDialog { get; set; }
        public ICommand AddReport { get; set; }
        public ICommand Debug { get; set; }
        public ICommand DownloadAll { get; set; }
        public ICommand DownloadSelected { get; set; }
        public ICommand EditReport { get; set; }
        public ICommand RemoveReport { get; set; }
        public Report SelectedItem { get; set; }

        public ReportVM()
        {
            AddReport = new AddReportCommand(this);
            Debug = new DebugCommand(this);
            DownloadAll = new DownloadAllCommand(this);
            DownloadSelected = new DownloadSelectedCommand(this);
            EditReport = new EditReportCommand(this);
            RemoveReport = new RemoveReportCommand(this);
            this.Status = "Ready.";
        }

        public string Payload
        {
            get { return payload; }
            set
            {
                payload = value;
                OnPropertyChanged("Payload");
            }
        }

        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        public void SaveReportList()
        {
            try
            {
                new FileInfo(reportFile).Directory.Create();
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ObservableCollection<Report>));
                using (StreamWriter streamWriter = new StreamWriter(reportFile))
                {
                    xmlSerializer.Serialize(streamWriter, ReportList);
                }
            }
            catch
            {
                string title = "Error: Data folder is not accessible.";
                string message = reportFile + "does not exist and cannot be created.";
                MessageBoxButton buttons = MessageBoxButton.OK;
                MessageBox.Show(message, title, buttons);
            }
        }
    }
}