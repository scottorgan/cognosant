using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CognosantGUI.Views;
using CognosantGUI.ViewModels;


namespace CognosantGUI.Commands
{
    class EditReportCommand : ICommand
    {
        private readonly ReportVM reportVM;
        
        public EditReportCommand(ReportVM viewModel)
        {
            reportVM = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return (reportVM.SelectedItem != null);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            var selectedItem = reportVM.SelectedItem;
            var sourcePath = reportVM.SelectedItem.Path;

            DetailsView view = new DetailsView(reportVM);
            view.DataContext = reportVM;
            view.Owner = Application.Current.MainWindow;
            view.Title = "Edit Report";
            view.UrlBox.IsEnabled = false;
            view.PasteLink.IsEnabled = false;
            view.ShowDialog();
            if (view.DialogResult == true)
            {
                reportVM.SaveReportList();
            }
            else
            {
                reportVM.SelectedItem.Path = sourcePath;
            }
        }
    }
}
