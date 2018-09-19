using System;
using System.Windows;
using System.Windows.Input;
using CognosantGUI.ViewModels;

namespace CognosantGUI.Commands
{
    class DebugCommand : ICommand
    {
        private ReportVM reportVM;

        public DebugCommand(ReportVM viewModel)
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
            reportVM.DebugMode = true;
            Views.DebugWindow view = new Views.DebugWindow(reportVM);
            view.DataContext = reportVM;
            view.Owner = Application.Current.MainWindow;
            view.Title = "Debug Window";
            view.Show();
        }
    }
}
