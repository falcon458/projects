using System.Configuration;

namespace SeriesUI.Configuration
{
    public class SaveFile : ConfigurationElement
    {
        [ConfigurationProperty("Name")] public string Name => (string) base["Name"];
    }
}