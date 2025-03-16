using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using InoriDock.WPF.Public;
using InoriDock.WPF.Public.Methods;
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