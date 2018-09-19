using System.Windows;
using CognosantGUI.ViewModels;

namespace CognosantGUI.Views
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        private readonly ReportVM reportVM;

        public DebugWindow(ReportVM viewModel)
        {
            InitializeComponent();
            reportVM = viewModel;
        }
    }
}
