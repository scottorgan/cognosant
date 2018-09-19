using Common.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace Common.ViewModels
{
    public class ReportBaseVM : INotifyPropertyChanged
    {
        protected ObservableCollection<Report> reportList;
        public event PropertyChangedEventHandler PropertyChanged;

        protected readonly string reportFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Cognosant\Reports.xml";

        public ReportBaseVM()
        {
            reportList = new ObservableCollection<Report>();
            LoadReportList();
        }

        public ObservableCollection<Report> ReportList
        {
            get { return reportList; }
            set
            {
                reportList = value;
                OnPropertyChanged("ReportList");
            }
        }

        private void LoadReportList()
        {
            if (File.Exists(reportFile))
            {
                try
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ObservableCollection<Report>));
                    using (StreamReader streamReader = new StreamReader(reportFile))
                    {
                        ReportList = xmlSerializer.Deserialize(streamReader) as ObservableCollection<Report>;
                    }
                    OnPropertyChanged("ReportList");
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
