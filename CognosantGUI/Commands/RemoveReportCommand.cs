using System;
using System.Windows.Input;
using CognosantGUI.ViewModels;

namespace CognosantGUI.Commands
{
    public class RemoveReportCommand : ICommand
    {
        private readonly ReportVM reportVM;

        public RemoveReportCommand(ReportVM viewModel)
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
            if (reportVM.ConfirmationDialog("Remove \"" + reportVM.SelectedItem.Name + "\" from Report List?", "Confirm"))
            {
                reportVM.ReportList.Remove(selectedItem);
            }

        }
    }
}
