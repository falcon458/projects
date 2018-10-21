using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SeriesUI.Configuration
{
    public class SeriesWebSiteLocation : ConfigurationElement
    {
        [ConfigurationProperty("suburl")]
        public string url
        {
            get => (string)base["suburl"];
        }
    }
}
