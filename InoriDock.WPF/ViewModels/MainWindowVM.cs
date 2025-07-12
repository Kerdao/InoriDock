using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Config.Net;
using InoriDock.WPF.Components.DockComponent;
using InoriDock.WPF.Components.DockComponent.DockItems;
using InoriDock.WPF.Services.Config.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Dock = InoriDock.WPF.Components.DockComponent.Dock;
using Str = InoriDock.WPF.Struct;
using InoriDock.WPF;

namespace InoriDock.WPF.ViewModels
{
    public partial class MainWindowVM : ObservableObject
    {
        private readonly Window _window;

        [ObservableProperty]
        private string _title = string.Empty;

        private Panel _dock;
        private DockObject _dockObject;

        public ICommand WindowLoadedCommond { get; private set; }
        public ICommand ContentRenderedCommond { get; private set; }
        public ICommand BorderPreviewDragOver { get; private set; }
        public ICommand BorderDrop { get; private set; }
        public ICommand MenuItemClick { get; private set; }

        public MainWindowVM(Window window) 
        {
            _window = window;

            WindowLoadedCommond = new RelayCommand<Object?>(OnWindowLoaded);
            ContentRenderedCommond = new RelayCommand<Object?>(OnContentRenderedCommond);
            BorderPreviewDragOver = new RelayCommand<Object?>(OnBorderPreviewDragOver);
            BorderDrop = new RelayCommand<Object?>(OnBorderDrop);
            MenuItemClick = new RelayCommand<dynamic>(OnMenuItemClick);
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
        private void OnContentRenderedCommond(object? obj)
        {
            _dock = (Panel)obj;
            _dockObject = Dock.GetDockObject((DependencyObject)obj);
        }

        //鼠标拖动状态进入范围
        private void OnBorderPreviewDragOver(Object? parameter)
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

            //阻止DragOver替换e.Effects
            e.Handled = true;
        }
        //鼠标拖动落下
        private void OnBorderDrop(Object? parameter)
        {
            if (parameter is DragEventArgs e == false) return;

            // 检查拖动的数据是否包含文件
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string path in paths)
                {
                    
                    MessageBox.Show($"文件路径: {path}");
                    /*
                     * 待修正
                     * 转变为对应的luk，url
                     *
                     */
                    switch (Path.GetExtension(path))
                    {
                        case ".lnk":
                            // 处理 .lnk 文件
                            //var icon = IconUtilities.ExtractIcon(path, IconSize.Jumbo);
                            var lnk = Methods.ReadShortcut(path);
                            var item = new LnkItem
                            {
                                DockOf = _dock,
                                TargetPath = lnk.TargetPath,
                                Arguments = lnk.Arguments,
                                Description = lnk.Description,
                                //IconLocation = lnk.IconLocation,
                                WindowStyle = lnk.WindowStyle,
                                WorkingDirectory = lnk.WorkingDirectory
                            };
                            _dockObject.Children.Add(item);
                            _dock.Children.Add(item);
                            break;
                        case ".url":
                            // 处理 .lnk 文件
                            //var icon = IconUtilities.ExtractIcon(path, IconSize.Jumbo);
                            //var item = new LnkItem
                            //{
                            //    DockOf = _dock,//待改善，改为构造传参
                            //    TargetPath = path,
                            //    Icon = icon
                            //};
                            //_dockObject.Children.Add(item);
                            //_dock.Children.Add(item);
                            break;
                        default:
                            break;
                    } 
                }
                return;
            }
        }

        private void OnMenuItemClick(dynamic valueObject)
        {
            Panel obj = valueObject.CommondParameter;
            DockObject dockObject = Dock.GetDockObject(obj);
            switch (valueObject.Header)
            {
                case "Save":
                    dockObject = Dock.GetDockObject(obj);
                    dockObject.Save();
                    break;
                case "Load":
                    dockObject = Dock.GetDockObject(obj);
                    dockObject.Load();
                    break;
            }
        }
    }
}
