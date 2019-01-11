using System.Configuration;

namespace SeriesUI.Configuration
{
    public class SeriesWebSite : ConfigurationElement
    {
        [ConfigurationProperty("url")] public string Url => (string) base["url"];
    }
}