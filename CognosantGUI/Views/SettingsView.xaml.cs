using System;
using System.Windows;
using Common;

namespace CognosantGUI.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            if (SettingsVM.Instance.UserSettings.Password != null)
            {
                UserPasswordBox.Password = IO.ToInsecureString(IO.DecryptString(SettingsVM.Instance.UserSettings.Password));
            }            
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (VerifiedRequiredFields())
            {
                // handle WPF's nullable boolean checkbox
                if (UrlCheckBox.IsChecked == true) { SettingsVM.Instance.UserSettings.OverwriteUrl = true; }
                else { SettingsVM.Instance.UserSettings.OverwriteUrl = false; }

                SettingsVM.Instance.UserSettings.Dsn = DsnBox.Text;
                SettingsVM.Instance.UserSettings.Password = IO.EncryptString(IO.ToSecureString(UserPasswordBox.Password));
                SettingsVM.Instance.UserSettings.Url = UrlBox.Text;
                SettingsVM.Instance.UserSettings.Username = UsernameBox.Text;
                SettingsVM.Instance.SaveSettings();
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (IO.VerifyUserSettings() == false)
            {
                string message = "Cognosant cannot run without these required settings and will now close.";
                string title = "Error";
                MessageBoxButton buttons = MessageBoxButton.OK;
                MessageBox.Show(message, title, buttons);
                Application.Current.Shutdown();
            }
            else { this.Close(); }
        }

        private bool VerifiedRequiredFields()
        {
            if (string.IsNullOrWhiteSpace(UsernameBox.Text))
            {
                MessageBox.Show("Username is a required setting.", "Error", MessageBoxButton.OK);
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UserPasswordBox.Password))
            {
                MessageBox.Show("Password is a required setting.", "Error", MessageBoxButton.OK);
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UrlBox.Text))
            {
                MessageBox.Show("Web Address is a required setting.", "Error", MessageBoxButton.OK);
                return false;
            }
            else if (string.IsNullOrWhiteSpace(DsnBox.Text))
            {
                MessageBox.Show("Data Source Name is a required setting." + Environment.NewLine + "See Help for more information.", "Error", MessageBoxButton.OK);
                return false;
            }
            else { return true; }
        }
    }
}
