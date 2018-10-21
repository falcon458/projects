using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
