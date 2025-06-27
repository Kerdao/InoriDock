using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace InoriDock.WPF.Components.DockComponent.DockItems
{
    //此类用于定义基础DockItem应具有基础属性和行为
    public class DockItem : DockItemBase
    {
        public virtual string Type { get; }

        public virtual JObject ToJObject()
        {
            return new JObject
            {
                ["Type"] = this.GetType().FullName
            };
        }

        public virtual void LoadFromJObject(JObject jObject)
        {
            
        }
    }
}
