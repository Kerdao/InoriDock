using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
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
            
        }

        public override JObject ToJObject()
        {
            var obj = base.ToJObject();
            obj["TargetPath"] = TargetPath;
            return obj;
        }

        public override void LoadFromJObject(JObject jObject)
        {
            TargetPath = jObject[nameof(TargetPath)]?.ToString();
        }
    }
}