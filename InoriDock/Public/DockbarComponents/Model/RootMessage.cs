using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InoriDock.Public.DockbarComponents.Model
{
    public class RootMessage
    {
        /*
        信息的版本
        list
           
           目标路径

         */

        public string Version {  get; set; }
        public List<ItemMessage> Items { get; set; }
    }
}
