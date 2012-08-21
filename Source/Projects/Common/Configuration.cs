using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Common
{
    public class Configuration
    {
        private string _username;
        public string Username
        {
            get
            {
                var container = GetApplicationcContainer();

                _username = container.Values["Username"].ToString();
                
                return _username;
            }
            set
            {
                var container = GetApplicationcContainer();

                _username = value;

                container.Values["Username"] = _username;
            }
        }

        private static ApplicationDataContainer GetApplicationcContainer()
        {
            var container = ApplicationData.Current.LocalSettings.Containers["FatCatReader"];

            if (container == null)
            {
                ApplicationData.Current.LocalSettings.CreateContainer("FatCatReader", ApplicationDataCreateDisposition.Always);
                container = ApplicationData.Current.LocalSettings.Containers["FatCatReader"];
            }
            return container;
        }
    }
}
