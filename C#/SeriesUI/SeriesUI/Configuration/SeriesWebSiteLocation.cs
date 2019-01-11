using System.Configuration;

namespace SeriesUI.Configuration
{
    public class SeriesWebSiteLocation : ConfigurationElement
    {
        [ConfigurationProperty("suburl")]
        public string Url
        {
            get => (string)base["suburl"];
        }
    }
}
