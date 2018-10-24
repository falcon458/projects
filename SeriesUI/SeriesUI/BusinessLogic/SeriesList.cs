using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SeriesUI.Configuration;

namespace SeriesUI.BusinessLogic
{
    internal class SeriesList
    {
        private readonly ConfigurationService configurationService;

        public SeriesList(ConfigurationService configurationService)
        {
            Series = new List<Series>();

            this.configurationService = configurationService;
        }

        public List<Series> Series { get; private set; }

        public void ReloadFromDisk()
        {
            var fileName = AppDomain.CurrentDomain.BaseDirectory + configurationService.SaveFile.Name;

            // Create the formatter to serialize the object.
            IFormatter formatter = new BinaryFormatter();

            // Create the stream that the serialized data will be buffered too.
            var buffer = File.OpenRead(fileName);

            // Invoke the Deserialize method. This yields a new Series object
            Series = formatter.Deserialize(buffer) as List<Series>;

            // Close the stream.
            buffer.Close();
        }

        public void SaveToDisk()
        {
            // Save application
            IFormatter formatter = new BinaryFormatter();
            var buffer = File.Create(AppDomain.CurrentDomain.BaseDirectory + configurationService.SaveFile.Name);

            formatter.Serialize(buffer, Series);
            buffer.Close();
        }

        public void Refresh()
        {
            ClearSeries();

            var placeHolder = configurationService.SeriesPlaceHolder.text;

            foreach (SeriesConfigElement newSeries in configurationService.SeriesConfigCollection)
            {
                var series = new Series
                {
                    Name = newSeries.Name,
                    WebSite = configurationService.SeriesWebSite.url,
                    LocalUrl = configurationService.SeriesWebSiteLocation.url.Replace(placeHolder,
                        newSeries.Name.Replace(" ", "-").ToLower())
                };

                series.GetDataFromWebsite();
                Series.Add(series);
            }
        }

        public void ClearSeries()
        {
            Series.Clear();
        }
    }
}