using System;
using System.Collections.Specialized;
using System.Configuration;

namespace SeriesUI.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly NameValueCollection appSettings;
        private readonly CustomApplicationConfigSection configSection;

        public ConfigurationService()
        {
            try
            {
                configSection =
                    ConfigurationManager.GetSection(CustomApplicationConfigSection.SectionName) as
                        CustomApplicationConfigSection;

                appSettings = ConfigurationManager.AppSettings;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #region IConfigurationService Members

        public string LabelInActiveColor => appSettings["LabelInActiveColor"];
        public string LabelActiveColor => appSettings["LabelActiveColor"];

        public int LabelWidth
        {
            get
            {
                if (int.TryParse(appSettings["LabelWidth"], out var result))
                    return result;
                throw new ArgumentException();
            }
        }

        public int LabelHeight
        {
            get
            {
                if (int.TryParse(appSettings["LabelHeight"], out var result))
                    return result;
                throw new ArgumentException();
            }
        }

        public int LabelSpacing
        {
            get
            {
                if (int.TryParse(appSettings["LabelSpacing"], out var result))
                    return result;
                throw new ArgumentException();
            }
        }

        public SeriesConfigCollection SeriesConfigCollection => configSection?.SeriesList;

        public SeriesWebSite SeriesWebSite => configSection?.SeriesWebSite;

        public SeriesPlaceHolder SeriesPlaceHolder => configSection?.SeriesPlaceHolder;

        public SeriesWebSiteLocation SeriesWebSiteLocation => configSection?.SeriesWebSiteLocation;

        #endregion
    }
}