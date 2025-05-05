using System.Drawing;
using Method = InoriDock.WPF.Methods.Methods;

namespace InoriDock.WPF.Components.DockComponent.DockItem
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

        public LnkItem()
        {
            
        }
    }
}