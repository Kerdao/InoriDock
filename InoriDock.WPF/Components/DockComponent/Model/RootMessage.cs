namespace InoriDock.WPF.Components.DockComponent.Model
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
