using System.Configuration;

namespace SeriesUI.Configuration
{
    public class SeriesConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("Name")] public string Name => base["Name"] as string;
    }
}