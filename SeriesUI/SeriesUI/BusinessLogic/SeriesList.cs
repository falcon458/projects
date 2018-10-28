using System;
using System.Collections.ObjectModel;
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
            Series = new ObservableCollection<Series>();

            this.configurationService = configurationService;
        }

        public ObservableCollection<Series> Series { get; private set; }

        public void ReloadFromDisk()
        {
            var fileName = AppDomain.CurrentDomain.BaseDirectory + configurationService.SaveFile.Name;

            // Create the formatter to serialize the object.
            IFormatter formatter = new BinaryFormatter();

            // Create the stream that the serialized data will be buffered too.
            var buffer = File.OpenRead(fileName);

            // Invoke the Deserialize method. This yields a new Series object
            Series = formatter.Deserialize(buffer) as ObservableCollection<Series>;

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
            // Load the series into a new list of Series
            var newSeries = new ObservableCollection<Series>();

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
                newSeries.Add(series);
            }

            MergeListOfSeries(Series, newSeries);

            ClearSeries();

            // Move the series. Setting Series to newSeries would mess up binding with the listBox
            foreach (var series in newSeries) Series.Add(series);
        }

        private void MergeListOfSeries(ObservableCollection<Series> oldSeries, ObservableCollection<Series> newSeries)
        {
            foreach (var newSeriesEntry in newSeries)
            foreach (var newSeasonEntry in newSeriesEntry.Seasons)
            foreach (var newEpisodeEntry in newSeasonEntry.Episodes)
                // Combining these if statements gives "Possible NullRefException)
                if (oldSeries.FirstOrDefault(c => c.Name == newSeriesEntry.Name) is Series oldSeriesEntry)
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

        public void ClearSeries()
        {
            Series.Clear();
        }
    }
}