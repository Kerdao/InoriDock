using IWshRuntimeLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Method = InoriDock.WPF.Methods;

namespace InoriDock.WPF.Components.DockComponent.DockItems
{
    public class LnkItem : ShortcutItem
    {
        /// <summary>
        /// 目标路径
        /// </summary>
        public string TargetPath
        {
            get => field;
            set 
            {
                field = value;
                try
                {
                    var icon = IconUtilities.ExtractIcon(value, IconSize.Jumbo);
                    IcoSource = Method.IconToBitmapSource(icon);
                }
                catch (FileNotFoundException e)
                {
                    IcoSource = null;
                }
            }
        } = string.Empty;
        /// <summary>
        /// 启动参数
        /// </summary>
        public string Arguments { get; set; } = string.Empty;
        /// <summary>
        /// 备注
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// 图标路径
        /// </summary>
        //public string IconLocation { get; set; }
        /// <summary>
        /// 窗口显示方式
        /// </summary>
        public int WindowStyle { get; set; } = 0;
        /// <summary>
        /// 工作目录
        /// </summary>
        public string WorkingDirectory { get; set; }

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
            obj[nameof(TargetPath)] = TargetPath;
            obj[nameof(Arguments)] = Arguments;
            obj[nameof(Description)] = Description;
            obj[nameof(WindowStyle)] = WindowStyle;
            obj[nameof(WorkingDirectory)] = WorkingDirectory;
            return obj;
        }

        public override void LoadFromJObject(JObject jObject, Panel DockOf)
        {
            base.LoadFromJObject(jObject, DockOf);
            TargetPath = jObject[nameof(TargetPath)].ToString();
            TargetPath = jObject[nameof(Arguments)].ToString();
            TargetPath = jObject[nameof(Description)].ToString();
            TargetPath = jObject[nameof(WindowStyle)].ToString();
            TargetPath = jObject[nameof(WorkingDirectory)].ToString();

        }


    }
}