using Newtonsoft.Json.Linq;
using System.Windows.Controls;

namespace InoriDock.WPF.Components.DockComponent.DockItems
{
    public class FileItem : DockItem
    {

        public string OpenWith { get; set; } = "1234";

        public override JObject ToJObject()
        {
            return base.ToJObject();
        }
    }
}
