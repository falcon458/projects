using System.Configuration;

namespace SeriesUI.Configuration
{
    class CustomApplicationConfigSection: ConfigurationSection
    {
        public const string SectionName = "CustomApplicationConfig";

        [ConfigurationProperty("SeriesList")]
        public SeriesConfigCollection SeriesList => base["SeriesList"] as SeriesConfigCollection;

        [ConfigurationProperty("SeriesWebSite")]
        public SeriesWebSite SeriesWebSite => base["SeriesWebSite"] as SeriesWebSite;

        [ConfigurationProperty("SeriesWebSiteLocation")]
        public SeriesWebSiteLocation SeriesWebSiteLocation => base["SeriesWebSiteLocation"] as SeriesWebSiteLocation;

        [ConfigurationProperty("SeriesPlaceHolder")]
        public SeriesPlaceHolder SeriesPlaceHolder => base["SeriesPlaceHolder"] as SeriesPlaceHolder;

        [ConfigurationProperty("SaveFile")]
        public SaveFile SaveFile => base["SaveFile"] as SaveFile;

    }
}
