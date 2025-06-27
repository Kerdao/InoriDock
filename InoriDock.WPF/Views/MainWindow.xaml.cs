using InoriDock.WPF.Components.DockComponent.DockItems;
using InoriDock.WPF.ViewModels;
using Newtonsoft.Json;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            sp.ToJObject();
        }
    }
}