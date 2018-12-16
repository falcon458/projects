using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SeriesUI.Configuration;

namespace SeriesUI.BusinessLogic
{
    internal class SeriesList
    {
        private readonly IConfigurationService configurationService;

        public SeriesList(IConfigurationService configurationService)
        {
            Series = new List<Series>();

            this.configurationService = configurationService;
        }

        public List<Series> Series { get; private set; }

        public event EventHandler<EventArgs> SeriesChanged;

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
            // Store the series into a new list of Series
            var oldSeries = new List<Series>();

            // Move the series to the new list
            foreach (var series in Series) oldSeries.Add(series);

            // Empty the old list
            ClearSeries();

            var placeHolder = configurationService.SeriesPlaceHolder.text;

            foreach (SeriesConfigElement newSeriesConfig in configurationService.SeriesConfigCollection)
            {
                var series = new Series
                {
                    Name = newSeriesConfig.Name,
                    WebSite = configurationService.SeriesWebSite.url,
                    LocalUrl = configurationService.SeriesWebSiteLocation.url.Replace(placeHolder,
                        newSeriesConfig.Name.Replace(" ", "-").ToLower())
                };

                series.GetDataFromWebsite();

                MergeListOfSeries(oldSeries, series);

                Series.Add(series);
                OnSeriesChanged(series, EventArgs.Empty);
            }
        }

        private void MergeListOfSeries(List<Series> oldSeriesList, Series newSeries)
        {
            foreach (var newSeasonEntry in newSeries.Seasons)
            foreach (var newEpisodeEntry in newSeasonEntry.Episodes)
                // Combining these if statements gives "Possible NullRefException)
                if (oldSeriesList.FirstOrDefault(c => c.Name == newSeries.Name) is Series oldSeriesEntry)
                    if (oldSeriesEntry.Seasons.FirstOrDefault(d => d.Sequence == newSeasonEntry.Sequence) is Season
                        oldSeasonEntry)
                        if (oldSeasonEntry.Episodes.FirstOrDefault(e => e.Number == newEpisodeEntry.Number) is Episode
                            oldEpisodeEntry)
                        {
                            newEpisodeEntry.Downloaded = oldEpisodeEntry.Downloaded;
                            newEpisodeEntry.SubTitleEn = oldEpisodeEntry.SubTitleEn;
                            newEpisodeEntry.SubTitleNl = oldEpisodeEntry.SubTitleNl;
                        }
        }

        private void ClearSeries()
        {
            Series.Clear();
            OnSeriesChanged(this, EventArgs.Empty);
        }

        private void OnSeriesChanged(object sender, EventArgs e)
        {
            SeriesChanged?.Invoke(sender, e);
        }
    }
}