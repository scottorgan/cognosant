using System.ComponentModel;

namespace Common.Models
{
    public class Settings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string domain, dsn, url, password, username;
        private bool overwriteUrl;

        public string Domain
        {
            get
            {
                if (domain != null)
                {
                    return domain;
                } else
                {
                    return "APSCN";
                }
            }
            set
            {
                domain = value;
                OnPropertyChanged("Domain");
            }
        }

        public string Dsn
        {
            get { return dsn; }
            set
            {
                dsn = value;
                OnPropertyChanged("Dsn");
            }
        }

        public bool OverwriteUrl
        {
            get { return overwriteUrl; }
            set
            {
                overwriteUrl = value;
                OnPropertyChanged("OverwriteUrl");
            }
        }
   
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }

        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                OnPropertyChanged("Username");
            }
        }

        public string Url
        {
            get { return url; }
            set
            {
                url = value;
                OnPropertyChanged("Url");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
                SettingsVM.Instance.settingsChanged = true;
            }
        }
    }
}
