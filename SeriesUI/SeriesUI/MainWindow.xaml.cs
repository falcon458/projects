using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SeriesUI.BusinessLogic;
using SeriesUI.Configuration;

// TODO:
// DataBinding --> Separate UI from code
// Refresh-merge (equals method?)
// save-file location
// add all remaining series
// Why do we need to refresh in btnAllNlSubs_Click to update the grid?
// Colors in grid & labels
// Ask when outstanding unsaved changes
// Databinding van Series, dan werkt Items.refresh tijdens serie-refresh

namespace SeriesUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ConfigurationService configurationService = new ConfigurationService();

        public MainWindow()
        {
            InitializeComponent();
        }

        // The current season (1-based)
        private int ActiveSeason { get; set; }

        private List<Series> SeriesList { get; set; }

        private void OnLabelMouseClick(object sender, EventArgs e)
        {
            // This construct checks for null value
            if (sender is Label label && int.TryParse(label.Content.ToString(), out var newLabelSeason))
            {
                ActiveSeason = newLabelSeason;

                UnMarkSeasonLabels();

                label.Background =
                    (Brush) new BrushConverter().ConvertFrom(configurationService.LabelActiveColor);

                if (listBoxSeries.SelectedItem is Series series &&
                    int.TryParse(label.Content.ToString(), out var season))
                    grdEpisodes.ItemsSource = series.Seasons[season - 1].Episodes;
            }
        }

        private void DeleteSeasonLabels()
        {
            foreach (var obj in grdSeasons.Children.OfType<Label>().Where(c => (c.Tag ?? "").ToString() == "temporary")
                .ToList()) grdSeasons.Children.Remove(obj);

            ActiveSeason = 0;
        }

        private void UnMarkSeasonLabels()
        {
            foreach (var item in grdSeasons.Children)
                if (item is Label && int.TryParse((item as Label).Content.ToString(), out var labelSeason) &&
                    labelSeason != ActiveSeason)
                    (item as Label).Background =
                        (Brush) new BrushConverter().ConvertFrom(configurationService.LabelInActiveColor);
        }

        private void OnLabelMouseEnter(object sender, EventArgs e)
        {
            // This construct checks for null value
            if (sender is Label label && int.TryParse(label.Content.ToString(), out var labelSeason) &&
                labelSeason != ActiveSeason)
                label.Background =
                    (Brush) new BrushConverter().ConvertFrom(configurationService.LabelActiveColor);
        }

        private void OnLabelMouseLeave(object sender, EventArgs e)
        {
            // This construct checks for null value
            if (sender is Label label && int.TryParse(label.Content.ToString(), out var labelSeason) &&
                labelSeason != ActiveSeason)
                label.Background =
                    (Brush) new BrushConverter().ConvertFrom(configurationService.LabelInActiveColor);
        }

        private void listBoxSeries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DeleteSeasonLabels();
            grdEpisodes.ItemsSource = null;

            var series = (sender as ListBox)?.SelectedItem as Series;

            for (var i = 0; i < series?.Seasons.Count; i++)
            {
                var lbl = GetNewLabel(i);

                grdSeasons.Children.Add(lbl);
            }

            // Select last season
            if (grdSeasons.Children.Count > 0)
                OnLabelMouseClick(grdSeasons.Children[grdSeasons.Children.Count - 1], new EventArgs());
        }

        private Label GetNewLabel(int sequence)
        {
            var labelWidth = configurationService.LabelWidth;
            var labelHeight = configurationService.LabelHeight;
            var labelSpacing = configurationService.LabelSpacing;

            var label = new Label
            {
                Width = labelWidth,
                Height = labelHeight,
                Content = sequence + 1,
                Background = (Brush) new BrushConverter().ConvertFrom(configurationService.LabelInActiveColor),
                BorderThickness = new Thickness(0),
                BorderBrush = Brushes.Black,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(sequence * (labelWidth + 1) + labelSpacing, 5, 0, 0),
                Tag = "temporary"
            };

            label.MouseEnter += OnLabelMouseEnter;
            label.MouseLeave += OnLabelMouseLeave;
            label.MouseLeftButtonDown += OnLabelMouseClick;

            return label;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            listBoxSeries.Items.Clear();
            listBoxSeries.Items.Refresh();

            SeriesList = new List<Series>();

            try
            {
                Cursor = Cursors.Wait;
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
                    listBoxSeries.Items.Add(series);
                    SeriesList.Add(series);
                }
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Save application
            IFormatter formatter = new BinaryFormatter();
            var buffer = File.Create(@"C:\temp\series.txt");
            formatter.Serialize(buffer, SeriesList);
            buffer.Close();
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            // Create the formatter to serialize the object.
            IFormatter formatter = new BinaryFormatter();

            // Create the stream that the serialized data will be buffered too.
            var buffer = File.OpenRead(@"C:\temp\series.txt");

            // Invoke the Deserialize method.
            SeriesList = formatter.Deserialize(buffer) as List<Series>;

            // Close the stream.
            buffer.Close();

            listBoxSeries.Items.Clear();
            listBoxSeries.Items.Refresh();

            if (SeriesList != null)
                foreach (var series in SeriesList)
                    listBoxSeries.Items.Add(series);
        }

        /// <summary>
        ///     Set NL subs on for this season
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAllNlSubs_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxSeries.SelectedItems.Count > 0)
                ((Series) listBoxSeries.SelectedItems[0])?.Seasons[ActiveSeason - 1].SetAllSubs(Episode.SubTitle.NL);

            grdEpisodes.Items.Refresh();
        }

        /// <summary>
        ///     Set EN subs on for this season
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAllEnSubs_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxSeries.SelectedItems.Count > 0)
                ((Series) listBoxSeries.SelectedItems[0])?.Seasons[ActiveSeason - 1].SetAllSubs(Episode.SubTitle.EN);

            grdEpisodes.Items.Refresh();
        }

        private void btnAllDownloaded_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxSeries.SelectedItems.Count > 0)
                ((Series) listBoxSeries.SelectedItems[0])?.Seasons[ActiveSeason - 1].SetAllEpisodesDownloaded();

            grdEpisodes.Items.Refresh();
        }

        private void btnDebug_Click(object sender, RoutedEventArgs e)
        {
            //var series = configurationService.SeriesConfigCollection;

            //MessageBox.Show((series[0] as SeriesConfigElement).Name);

            //var tst = configurationService.SeriesConfigCollection;

            //foreach (var el in tst) MessageBox.Show(el.Name);
            listBoxSeries.Items.Clear();
            listBoxSeries.Items.Refresh();

            try
            {
                Cursor = Cursors.Wait;
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
                    listBoxSeries.Items.Add(series);
                }
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }
    }
}