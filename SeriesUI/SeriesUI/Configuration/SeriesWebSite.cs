using System.Configuration;

namespace SeriesUI.Configuration
{
    public class SeriesWebSite : ConfigurationElement
    {
        [ConfigurationProperty("url")] public string url => (string) base["url"];
    }
}