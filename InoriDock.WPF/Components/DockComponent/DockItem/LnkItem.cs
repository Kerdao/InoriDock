using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using Method = InoriDock.WPF.Methods;

namespace InoriDock.WPF.Components.DockComponent.DockItems
{
    public class LnkItem : ShortcutItem
    {
        /// <summary>
        /// 目标路径
        /// </summary>
        public string? TargetPath { get; set; }
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
            Click += (sender, e) =>
            {
                if (string.IsNullOrEmpty(TargetPath))
                {
                    MessageBox.Show("目标路径未设置！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                try
                {
                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = TargetPath,
                        UseShellExecute = true,
                        //Verb = "runas" // 以管理员身份运行（如果需要）
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"无法启动目标应用程序：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
        }

        public override JObject ToJObject()
        {
            var obj = base.ToJObject();
            obj["TargetPath"] = TargetPath;
            return obj;
        }

        public override void LoadFromJObject(JObject jObject, Panel DockOf)
        {
            base.LoadFromJObject(jObject, DockOf);
            TargetPath = jObject[nameof(TargetPath)]?.ToString();
            Icon = IconUtilities.ExtractIcon(TargetPath, IconSize.Jumbo);
        }

        
    }
}