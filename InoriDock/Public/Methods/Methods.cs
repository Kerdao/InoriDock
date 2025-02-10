using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InoriDock.Public.Methods
{
    class Methods
    {
        /// <summary>
        /// 计算鼠标与控件中心点之间的距离。
        /// </summary>
        /// <param name="control">目标控件</param>
        /// <param name="mousePosition">鼠标指针相对于控件的位置</param>
        /// <returns>在窗体内鼠标与控件中心点之间的距离。</returns>
        public static double DistanceOfMouseThanControl(object control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            // 确保 control 是 FrameworkElement 类型
            if (control is FrameworkElement frameworkElement && control is UIElement elementControl)
            {

                Point mousePosition = Mouse.GetPosition(elementControl);
                // 获取控件的中心点
                Point controlCenter = new Point(frameworkElement.ActualWidth / 2, frameworkElement.ActualHeight / 2);

                // 计算欧几里得距离
                double distance = Math.Sqrt(Math.Pow(mousePosition.X - controlCenter.X, 2) +
                                            Math.Pow(mousePosition.Y - controlCenter.Y, 2));

                return distance;
            }
            else
            {
                throw new InvalidOperationException("The control must be a FrameworkElement and UIElement.");
            }
        }
        /// <summary>
        /// 判断鼠标是否在指定控件上
        /// </summary>
        /// <param name="control">控件</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">空传参</exception>
        /// <exception cref="ArgumentException">传入类型或基类为FrameworkElement类型的对象</exception>
        public static bool IsMouseOverWindow(object control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }
            if (control is FrameworkElement frameworkElement)
            {
                Point mousePosition = Mouse.GetPosition(frameworkElement);
               if (mousePosition.X >= 0 && mousePosition.X <= frameworkElement.ActualWidth &&
                   mousePosition.Y >= 0 && mousePosition.Y <= frameworkElement.ActualHeight)
               {
                   // 鼠标在窗体内
                   return true;
               }
               else
               {
                   return false;
               }
            }else { throw new ArgumentException("The control must be a FrameworkElement.", nameof(control)); }
        }
        /// <summary>
        /// 获取控件在其父容器中的索引
        /// </summary>
        /// <param name="control">目标控件</param>
        /// <returns>控件在其父容器中的索引，如果父容器不支持索引，则返回 -1</returns>
        public static int GetControlIndexThanParent(object control)
        {
            // 使用模式匹配确保传入的对象是 UIElement
            if (control is UIElement controlElement)
            {
                // 获取控件的父容器
                var parent = VisualTreeHelper.GetParent(controlElement) as Panel;
                if (parent == null)
                {
                    // 如果父容器不是 Panel 类型，则返回 -1
                    return -1;
                }

                // 遍历父容器的子控件，找到目标控件的索引
                for (int i = 0; i < parent.Children.Count; i++)
                {
                    if (parent.Children[i] == controlElement)
                    {
                        return i; // 找到目标控件，返回其索引
                    }
                }
            }

            // 如果传入的对象不是 UIElement 或未找到目标控件，则返回 -1
            return -1;
        }
        //返回资源
        public static object GetResource(string uri, string key)
        {
            foreach (ResourceDictionary rd in Application.Current.Resources.MergedDictionaries)
            {
                if (rd.Source == new Uri(uri, UriKind.RelativeOrAbsolute))
                    return rd[key];
            }
            return null;
        }
        //返回资源字典
        public static ResourceDictionary GetResource(string uri)
        {
            foreach (ResourceDictionary rd in Application.Current.Resources.MergedDictionaries)
            {
                if (rd.Source == new Uri(uri, UriKind.RelativeOrAbsolute))
                    return rd;
            }
            return null;
        }
    }
    
}
