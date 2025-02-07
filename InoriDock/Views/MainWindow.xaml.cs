using System.Windows;
using System.Windows.Controls;
using InoriDock.Public;
using InoriDock.ViewModels;

namespace InoriDock.Views
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