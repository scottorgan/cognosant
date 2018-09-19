using System;
using System.Windows;
using System.Windows.Input;
using CognosantGUI.Utilities;
using CognosantGUI.Views;
using CognosantGUI.ViewModels;
using Common.Models;

namespace CognosantGUI.Commands
{
    class AddReportCommand : ICommand
    {
        private readonly ReportVM reportVM;

        public AddReportCommand(ReportVM viewModel)
        {
            reportVM = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            var newReport = new Report();
            reportVM.ReportList.Add(newReport);
            reportVM.SelectedItem = newReport;

            DetailsView view = new DetailsView(reportVM);
            view.DataContext = reportVM;
            view.Owner = Application.Current.MainWindow;
            view.Title = "Add New Report";
            view.ShowDialog();

            if (view.DialogResult == true)
            {
                reportVM.SelectedItem.Name = GuiIO.GetReportName(reportVM.SelectedItem.Url);
                reportVM.SelectedItem.Enabled = true;
                reportVM.SaveReportList();
            }
            else
            {
                reportVM.ReportList.Remove(reportVM.SelectedItem);
            }
        }
    }
}
