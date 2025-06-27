using Config.Net;
using Config.Net.Stores;
using InoriDock.WPF.Services.Config.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InoriDock.WPF.Services.Config.Helper
{
    public static class ConfigurationExtensionsHelper
    {
        public static ConfigurationBuilder<TInterface> UseJsonFileHelper<TInterface>(this ConfigurationBuilder<TInterface> builder, string jsonFilePath) where TInterface : class
        {
            builder.UseConfigStore(new JsonConfigStoreHelper(jsonFilePath, isFilePath: true));
            return builder;
        }
    }
}
