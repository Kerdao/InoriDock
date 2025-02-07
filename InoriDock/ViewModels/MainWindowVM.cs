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

namespace InoriDock.ViewModels
{
    public partial class MainWindowVM : ObservableObject
    {
        private readonly Window _window;

        [ObservableProperty]
        private string _title = string.Empty;

        public ICommand WindowLoadedCommond { get; private set; }
        public ICommand MouseEnterCommond { get; private set; }
        public ICommand MouseLeaveCommond { get; private set; }

        public MainWindowVM(Window window) 
        {
            _window = window;
            WindowLoadedCommond = new RelayCommand<Object?>(OnWindowLoaded);
            MouseLeaveCommond = new RelayCommand<Object?>(OnMouseLeave);
            MouseEnterCommond = new RelayCommand<Object?>(OnMouseEnter);
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
        }
        private void OnMouseEnter(Object? parameter)
        {
            return;
            var button = parameter as Button;
            if (button != null)
            {
                var margin = button.Margin;
                button.Margin = new Thickness(20, 0, 20, 80);
            }
        }
        private void OnMouseLeave(Object? parameter)
        {
            return;
            var button = parameter as Button;
            if (button != null)
            {
                var margin = button.Margin;
                button.Margin = new Thickness(20, 0, 20, 0);
            }
        }
    }
}
