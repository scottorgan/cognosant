using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using Common.Models;

namespace Common
{
    class SettingsVM : INotifyPropertyChanged
    {
        private static readonly Lazy<SettingsVM> instance = new Lazy<SettingsVM>(() => new SettingsVM());
        public static SettingsVM Instance
        {
            get { return instance.Value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private Settings userSettings;

        private SettingsVM()
        {
            userSettings = new Settings();
            LoadSettings();
        }

        public Settings UserSettings
        {
            get { return userSettings; }
            set
            {
                userSettings = value;
                //OnPropertyChanged("UserSettings");
            }
        }

        internal bool settingsChanged = false;
        string configFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Cognosant\config.xml";

        public bool LoadSettings()
        {
            try
            {
                if (File.Exists(configFile))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
                    using (StreamReader streamReader = new StreamReader(configFile))
                    {
                        this.UserSettings = xmlSerializer.Deserialize(streamReader) as Settings;
                    }
                } else
                {
                    // config file does not exist... load suggested defaults
                    UserSettings.Domain = "APSCN";
                    UserSettings.Dsn = null;
                    UserSettings.OverwriteUrl = true;
                    UserSettings.Password = null;
                    UserSettings.Url = "https://adecognos.arkansas.gov";
                    UserSettings.Username = null;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return true;
        }

        public void SaveSettings()
        {
            if (settingsChanged)
            {
                try
                {
                    new FileInfo(configFile).Directory.Create();
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
                    using (StreamWriter streamWriter = new StreamWriter(configFile, false))
                    {
                        xmlSerializer.Serialize(streamWriter, Instance.UserSettings);
                    }
                    settingsChanged = false;
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
