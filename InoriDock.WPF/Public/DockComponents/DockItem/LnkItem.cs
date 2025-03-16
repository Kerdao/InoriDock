using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Method = InoriDock.WPF.Public.Methods.Methods;

namespace InoriDock.WPF.Public.DockComponents.DockItem
{
    public class LnkItem : ShortcutItem
    {
        /// <summary>
        /// 目标路径
        /// </summary>
        public string TargetPath { get; set; }

        private Icon _icon;
        public Icon Icon {
            get => _icon;
            set
            {
                _icon = value;
                Source = Method.IconToBitmapSource(value);
            }
        }

        public LnkItem(string TargetPath)
        {
            
        }
    }
}