using System.Windows;
using System.Windows.Controls;
using InoriDock.WPF.Public;
using InoriDock.WPF.ViewModels;

namespace InoriDock.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainWindowVM(this);
        }

    }
}