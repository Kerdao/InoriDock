using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using InoriDock.WPF.Components.DockComponent.DockItems;

namespace InoriDock.WPF.Components.DockComponent;

public class Dock
{
    private static object DockOwner;

    public static bool GetIsDockEnabled(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsDockEnabledProperty);
    }

    public static void SetIsDockEnabled(DependencyObject obj, bool value)
    {
        obj.SetValue(IsDockEnabledProperty, value);
    }

    //主控函数
    //为true后进行初始化和订阅，若出现一些改变需要重新设置关闭后再开启
    // Using a DependencyProperty as the backing store for IsDock.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IsDockEnabledProperty =
        DependencyProperty.RegisterAttached("IsDockEnabled", typeof(bool), typeof(Dock), new PropertyMetadata(
            false,
            (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                if (d is Panel panel)
                {
                    //如果是Panel类或派生类
                    
                    panel.Loaded += OnDockLoaded;
                    panel.MouseEnter += OnDockMouseEnter;
                    panel.MouseLeave += OnDockMouseLeave;
                }
                else
                {
                    // 如果不是 Panel 类或派生类，抛出异常
                    throw new InvalidOperationException(
                        "IsDockEnabled can only be applied to Panel or its derived classes.");
                }
            }));

    public static DockObject GetDockObject(DependencyObject obj)
    {
        return (DockObject)obj.GetValue(DockObjectProperty);
    }

    public static void SetDockObject(DependencyObject obj, DockObject value)
    {
        obj.SetValue(DockObjectProperty, value);
    }

    // Using a DependencyProperty as the backing store for DockObject.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DockObjectProperty =
        DependencyProperty.RegisterAttached("DockObject", typeof(DockObject), typeof(Dock));


    //static Dock()
    //{

    //}

    private static void OnDockLoaded(object sender, RoutedEventArgs e)
    {
        var panel = (Panel)sender;

        SetDockObject(panel, new DockObject(panel)
        {
            Children = new List<DockItem>()
        });

        foreach (object item in panel.Children)
        {
            if (item is DockItem dockItem)
            {
                dockItem.DockOf = panel;
                GetDockObject(panel).Children.Add(dockItem);
            }
        }
    }
    private static void OnDockMouseEnter(object sender, MouseEventArgs e)
    {
        //暂且不写
        //e.Handled = true;
    }
    private static void OnDockMouseLeave(object sender, MouseEventArgs e)
    {
        GetDockObject((DependencyObject)sender).MouseOverIndex = -1;
    }

    ///// <summary>
    ///// 刷新指定Dock(如有加入DockItem)
    ///// </summary>
    ///// <param name="obj"></param>
    //public static void Refresh(Panel panel)
    //{

    //}

    public static void ShowPreviewItem(int DockIdex, int InsertIndex)
    {

    }

}