using System.Configuration;

namespace SeriesUI.Configuration
{
    public class SeriesPlaceHolder : ConfigurationElement
    {
        [ConfigurationProperty("text")] public string Text => (string) base["text"];
    }
}