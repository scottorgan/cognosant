using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Common;
using CognosantGUI.Views;
using CognosantGUI.ViewModels;

namespace CognosantGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new ReportVM();
            this.DataContext = viewModel;

            viewModel.ConfirmationDialog = (message, title) => MessageBox.Show(message, title, MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var viewModel = (ReportVM)this.DataContext;
            viewModel.SaveReportList();
            SettingsVM.Instance.SaveSettings();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            if (IO.VerifyUserSettings() == false)
            {
                string message = "Cognosant is not fully configured.  Please review the required settings.";
                string title = "Error";
                MessageBoxButton buttons = MessageBoxButton.OK;
                MessageBox.Show(message, title, buttons);
                SettingsView settingsView = new SettingsView();
                settingsView.Owner = Application.Current.MainWindow;
                settingsView.ShowDialog();
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsView settingsView = new SettingsView();
            settingsView.Owner = Application.Current.MainWindow;
            settingsView.ShowDialog();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://scottorgan.net/cognosant.html");
        }
    }
}
