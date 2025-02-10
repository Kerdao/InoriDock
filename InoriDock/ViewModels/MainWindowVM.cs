using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading.Tasks;

using InoriDock.Public;
using System.Windows.Threading;
using InoriDock.Public.Methods;
using Str = InoriDock.Public.Methods.Struct;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace InoriDock.ViewModels
{
    public partial class MainWindowVM : ObservableObject
    {
        private readonly Window _window;

        [ObservableProperty]
        private string _title = string.Empty;
        [ObservableProperty]
        private ImageSource _source = new BitmapImage(new Uri("pack://application:,,,/Public/Icon/FailedImage.png"));

        public ICommand WindowLoadedCommond { get; private set; }
        public ICommand BorderDragEnter { get; private set; }
        public ICommand BorderDrop { get; private set; }

        public MainWindowVM(Window window) 
        {
            _window = window;
            WindowLoadedCommond = new RelayCommand<Object?>(OnWindowLoaded);
            BorderDragEnter = new RelayCommand<Object?>(OnBorderDragEnter);
            BorderDrop = new RelayCommand<Object?>(OnBorderDrop);
        }

        private void OnWindowLoaded(Object? parameter)
        {
            // 设置窗口位置为手动模式
            _window.WindowStartupLocation = WindowStartupLocation.Manual;

            // 获取屏幕的工作区域（排除任务栏等）
            var screen = System.Windows.SystemParameters.WorkArea;

            // 计算窗口的水平中心位置
            _window.Left = (screen.Width - _window.Width) / 2 + screen.Left;

            // 计算窗口的垂直位置（中下方）
            // 假设窗口距离屏幕底部的距离为屏幕高度的 1/4
            _window.Top = screen.Bottom - _window.Height - _window.Height / 20;

            //Task.Run(async () =>
            //{
            //    while (true)
            //    {
            //        Application.Current.Dispatcher.Invoke(() =>
            //        {
            //            Title = Methods.IsMouseOverWindow("_window").ToString();
            //        });
            //        await Task.Delay(500);
            //    }

            //});
            Str.ShortcutDescription shortcut = (Str.ShortcutDescription)Methods.ReadShortcut("C:\\Users\\xiaokedao\\Desktop\\PCL2.lnk");
            MessageBox.Show(shortcut.TargetPath);
        }

        //鼠标拖动状态进入范围
        private void OnBorderDragEnter(Object? parameter)
        {
            if (parameter is DragEventArgs e == false) return;

            // 检查拖动的数据是否包含文件
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy; // 允许复制操作
            }
            else
            {
                e.Effects = DragDropEffects.None; // 不允许操作
            }
        }
        //鼠标拖动落下
        private void OnBorderDrop(Object? parameter)
        {
            if (parameter is DragEventArgs e == false) return;

            // 检查拖动的数据是否包含文件
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    MessageBox.Show($"文件路径: {file}");

                    var a = IconUtilities.ExtractIcon(file, IconSize.Jumbo);
                    var b = Methods.IconToBitmapSource(a);
                    Source = b;
                }
            }
        }
    }
}
