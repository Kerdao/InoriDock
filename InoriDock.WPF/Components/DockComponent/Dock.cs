using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using InoriDock.WPF.Components.DockComponent.DockItem;

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

    public static int GetMouseOverIndex(DependencyObject obj)
    {
        return (int)obj.GetValue(MouseOverIndexProperty);
    }

    public static void SetMouseOverIndex(DependencyObject obj, int value)
    {
        obj.SetValue(MouseOverIndexProperty, value);
    }

    //处于鼠标悬停的索引
    // Using a DependencyProperty as the backing store for MouseOverIndex.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MouseOverIndexProperty =
        DependencyProperty.RegisterAttached("MouseOverIndex", typeof(int), typeof(Dock), new PropertyMetadata(
            -1,
            (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                if ((bool)GetIsDockEnabled(d))
                {
                    var panel = (Panel)d;
                    var overIndex = (int)e.NewValue;
                    int panclIndex = GetPanclIndex(d);


                    if ((int)e.NewValue != -1)
                    {
                        // >1if用于修复在没有触发DockMouseLeave事件情况下有多个Grade为1的item
                        if (Math.Abs((int)e.NewValue - (int)e.OldValue) > 1 && (int)e.NewValue != -1 && (int)e.OldValue != -1)
                        {
                            //MessageBox.Show("c  new:"+  (int)e.NewValue +" old:"+ (int)e.OldValue);
                            int oldV = (int)e.OldValue;
                            SetDockItemStyle(panclIndex, oldV - 3, -1);
                            SetDockItemStyle(panclIndex, oldV - 2, -1);
                            SetDockItemStyle(panclIndex, oldV - 1, -1);
                            SetDockItemStyle(panclIndex, oldV, -1);
                            SetDockItemStyle(panclIndex, oldV + 1, -1);
                            SetDockItemStyle(panclIndex, oldV + 2, -1);
                            SetDockItemStyle(panclIndex, oldV + 3, -1);
                        }
                        SetDockItemStyle(panclIndex, overIndex - 3, 4);
                        SetDockItemStyle(panclIndex, overIndex - 2, 3);
                        SetDockItemStyle(panclIndex, overIndex - 1, 2);
                        SetDockItemStyle(panclIndex, overIndex, 1);
                        SetDockItemStyle(panclIndex, overIndex + 1, 2);
                        SetDockItemStyle(panclIndex, overIndex + 2, 3);
                        SetDockItemStyle(panclIndex, overIndex + 3, 4);
                    }
                    else
                    {
                        for (int i = 0; i < _dockItemList[GetPanclIndex(d)].Count; i++)
                        {
                            SetDockItemStyle(panclIndex, i, -1);
                        }
                    }
                }
                else
                {
                    //如果为false
                    //SetMouseOverIndex(d, -1);
                    throw new InvalidOperationException(
                        "IsDockEnabled must be set to true before using MouseOverIndex.");
                }
            }));


    //PanclIndex,对于多个Dock的标识
    public static int GetPanclIndex(DependencyObject obj)
    {
        return (int)obj.GetValue(PanclIndexProperty);
    }

    public static void SetPanclIndex(DependencyObject obj, int value)
    {
        obj.SetValue(PanclIndexProperty, value);
    }

    // Using a DependencyProperty as the backing store for PanclIndex.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PanclIndexProperty =
        DependencyProperty.RegisterAttached("PanclIndex", typeof(int), typeof(Dock), new PropertyMetadata(-1));


    private static List<Panel> _dockList;
    private static List<List<DockItemBase>> _dockItemList;

    public static DockList GetDockList
    {
        get
        {
            return new DockList(_dockList, _dockItemList);
        }
    }

    static Dock()
    {
        _dockList = [];
        _dockItemList = [];
    }

    private static void SetDockItemStyle(int panelIndex , int index, int Grade)
    {
        //因为index序列从0开始
        if (index < 0 || index + 1 > _dockItemList[panelIndex].Count)
        {
            //筛选器
            return;
        }
        
            _dockItemList[panelIndex][index].BouncingAnimation(Grade);

        
                      
    }
    private static void OnDockLoaded(object sender, RoutedEventArgs e)
    {
        var panel = (Panel)sender;

        SetPanclIndex(panel, _dockList.Count);
        _dockList.Add(panel);
        _dockItemList.Add(new List<DockItemBase>());

        int index = 0;
        foreach (object item in panel.Children)
        {
            if (item is DockItemBase dockItem)
            {
                dockItem.Index = index;
                index += 1;


                _dockItemList[GetPanclIndex(panel)].Add(dockItem);

                dockItem.MouseEnter += OnDockItemMouseEnter;
                dockItem.MouseLeave += OnDockItemMouseLeave;
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
        SetMouseOverIndex((DependencyObject)sender, -1);
    }
    private static void OnDockItemMouseEnter(object sender, MouseEventArgs e)
    {
        var uI = (UIElement)sender;
        var button = (DockItemBase)sender;
        //MessageBox.Show(sender.ToString()+button.Index);

        SetMouseOverIndex(VisualTreeHelper.GetParent(uI), button.Index);

        e.Handled = true;
    }
    private static void OnDockItemMouseLeave(object sender, MouseEventArgs e)
    {
        //暂且不写
        e.Handled = true;
    }


    /// <summary>
    /// 刷新全部Dock(如有加入新的Dock或及DockItem)
    /// </summary>
    [Obsolete("此方法尚未完成，功能暂定，请使用重载方法。", true)]
    public static void Refresh()
    {
        return;
        //暂定
        //清除所有dockItem
        foreach (List<DockItemBase> d in _dockItemList)
        {
            foreach (DockItemBase item in d)
            {
                item.MouseEnter -= OnDockItemMouseEnter;
                item.MouseLeave -= OnDockItemMouseLeave;
            }
        }
        foreach (var item in _dockList)
        {
            SetMouseOverIndex(item, -1);
            item.Loaded -= OnDockLoaded;
            item.MouseEnter -= OnDockMouseEnter;
            item.MouseLeave -= OnDockMouseLeave;
        }
        _dockItemList.Clear();
        _dockList.Clear();
    }

    /// <summary>
    /// 刷新指定Dock(如有加入DockItem)
    /// </summary>
    /// <param name="obj"></param>
    public static void Refresh(int panelInt)
    {
        Panel panel = _dockList[panelInt];

        //为子元素事件订阅和记录子元素中的所有dockItem
        foreach (DockItemBase item in _dockItemList[GetPanclIndex(panel)])
        {
            item.MouseEnter -= OnDockItemMouseEnter;
            item.MouseLeave -= OnDockItemMouseLeave;
        }
        _dockItemList[GetPanclIndex(panel)].Clear();

        int index = 0;
        foreach (var item in panel.Children)
        {
            if (item is DockItemBase dockItem)
            {
                dockItem.Index = index;
                index += 1;

                _dockItemList[GetPanclIndex(panel)].Add(dockItem);
                dockItem.MouseEnter += OnDockItemMouseEnter;
                dockItem.MouseLeave += OnDockItemMouseLeave;
            }
        }
    }
    public static void AddItem(int DockIdex, DockItemBase item)
    {
        //没完善
        Panel panel = _dockList[DockIdex];

        panel.Children.Add(item);
        _dockItemList[DockIdex].Add(item);
    }
    public static void AddItem(int DockIdex, DockItemBase item,int InsertIndex)
    {
        //没完善
        Panel panel = _dockList[DockIdex];

        panel.Children.Add(item);
        _dockItemList[DockIdex].Insert(InsertIndex,item);
    }
    public static void RemoveItem(int DockIdex, int Index)
    {
        //没完善
        Panel panel = _dockList[DockIdex];

        DockItemBase item = _dockItemList[DockIdex][Index];
        panel.Children.Remove(item);
        _dockItemList[DockIdex].RemoveAt(Index);
    }

    public static void ShowPreviewItem(int DockIdex, int InsertIndex)
    {

    }

}