using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using CognosantGUI.Utilities;
using CognosantGUI.ViewModels;

namespace CognosantGUI.Views
{
    /// <summary>
    /// Interaction logic for DetailsView.xaml
    /// </summary>
    public partial class DetailsView : Window
    {
        private readonly ReportVM reportVM;

        public DetailsView(ReportVM viewModel)
        {
            InitializeComponent();
            reportVM = viewModel;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.ValidateNames = false;
            dialog.CheckFileExists = false;
            dialog.CheckPathExists = false;
            dialog.FileName = " ";
            bool? result = dialog.ShowDialog();
            if ((result.HasValue) && (result.Value))
            {
                if (Path.GetFileName(dialog.FileName) == " ")
                {
                    this.BrowseTextBox.Text = Path.GetDirectoryName(dialog.FileName);
                }
                else
                {
                    this.BrowseTextBox.Text = dialog.FileName.Replace(" ", string.Empty);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string path;

            // Make sure the download path exists.    
            try
            {
                path = GuiIO.VerifyDownloadPath(BrowseTextBox.Text);

                if (Path.GetFileName(path) == "download.txt")
                {
                    string filePath = Path.GetDirectoryName(path);
                    string fileName = GuiIO.GetReportName(UrlBox.Text);
                    string fileExt = GuiIO.GetReportFormat(UrlBox.Text);

                    path = filePath + @"\" + fileName + "." + fileExt;
                }

                if (GuiIO.FileNameValid(Path.GetFileName(path)))
                {
                    BrowseTextBox.Text = path;
                    DialogResult = true;
                    this.Close();
                }
                else
                {
                    throw new Exception("File name contains invalid characters.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Oops!", MessageBoxButton.OK);
            }
        }

        private void PasteLink_Click(object sender, RoutedEventArgs e)
        {
            UrlBox.Text = Clipboard.GetText();
        }
    }
}
