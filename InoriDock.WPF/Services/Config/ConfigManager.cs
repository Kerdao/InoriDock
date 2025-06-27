using Config.Net;
using InoriDock.WPF.Services.Config.Helper;
using InoriDock.WPF.Services.Config.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InoriDock.WPF.Services.ConfigTool
{
    public class ConfigManager
    {
        public readonly SettingModel setting = new ConfigurationBuilder<SettingModel>()
            .UseJsonFileHelper("config/settings")
            .Build();

    }

}
