using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using SeriesUI.BusinessLogic;
using SeriesUI.Configuration;
using SeriesUI.Windows;

// TODO: Andere kleuren? Omranden?
// Refresh-merge (equals method?)
// Why do we need to refresh in btnAllNlSubs_Click (and others) to update the grid?
// Colors in grid
// Ask when outstanding unsaved changes (INotifyPropertyChanged)
// Log to file
// Better Call Saul - S04E06 - PiÃ±ata

namespace SeriesUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<int, Brush> colorPalette;
        private readonly IConfigurationService configurationService;
        private readonly SeriesList seriesList;

        public MainWindow()
        {
            InitializeComponent();

            configurationService = new ConfigurationService();

            colorPalette = new Dictionary<int, Brush>();

            MyInitializeComponent();

            seriesList = new SeriesList(configurationService);

            listBoxSeries.ItemsSource = seriesList.Series;
        }

        // The current season (1-based)
        private int ActiveSeason { get; set; }

        private void MyInitializeComponent()
        {
            // Initialize the Color Palette
            colorPalette[new ColorPaletteKey(CompletenessState.Complete, true).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveCompleteColor);
            colorPalette[new ColorPaletteKey(CompletenessState.Complete, false).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveCompleteColor);

            colorPalette[new ColorPaletteKey(CompletenessState.NotSubbedNl, true).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotSubbedNlColor);
            colorPalette[new ColorPaletteKey(CompletenessState.NotSubbedNl, false).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotSubbedNlColor);

            colorPalette[new ColorPaletteKey(CompletenessState.NotSubbed, true).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotSubbedColor);
            colorPalette[new ColorPaletteKey(CompletenessState.NotSubbed, false).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotSubbedColor);

            colorPalette[new ColorPaletteKey(CompletenessState.NotDownloaded, true).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotDownloadedColor);
            colorPalette[new ColorPaletteKey(CompletenessState.NotDownloaded, false).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotDownloadedColor);
        }

        private void OnLabelMouseClick(object sender, EventArgs e)
        {
            // This construct checks for null value
            if (sender is Label label && int.TryParse(label.Content.ToString(), out var newLabelSeason))
            {
                // Store the new active season
                ActiveSeason = newLabelSeason;

                // Un-mark all labels
                UnMarkSeasonLabels();

                // Set the background for this label
                label.Background = LabelBackGround(ActiveSeason - 1, true);

                // Show the episodes for this label's season
                if (listBoxSeries.SelectedItem is Series series)
                    grdEpisodes.ItemsSource = series.Seasons[ActiveSeason - 1].Episodes;
            }
        }

        private Brush LabelBackGround(int season, bool active)
        {
            Brush result = null;

            if (listBoxSeries.SelectedItem is Series series)
            {
                var key = new ColorPaletteKey(series.Seasons[season].Completeness, active)
                    .GetHashCode();

                result = colorPalette[key];
            }

            return result;
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
                if (item is Label label && int.TryParse(label.Content.ToString(), out var labelSeason) &&
                    labelSeason != ActiveSeason)
                    label.Background = LabelBackGround(labelSeason - 1, false);
        }

        private void OnLabelMouseEnter(object sender, EventArgs e)
        {
            // [!] This construct checks for null value
            if (sender is Label label && int.TryParse(label.Content.ToString(), out var labelSeason) &&
                labelSeason != ActiveSeason)
                label.Background = LabelBackGround(labelSeason - 1, true);
        }

        private void OnLabelMouseLeave(object sender, EventArgs e)
        {
            if (sender is Label label && int.TryParse(label.Content.ToString(), out var labelSeason) &&
                labelSeason != ActiveSeason)
                label.Background = LabelBackGround(labelSeason - 1, false);
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
            // [!] No need to set the background here since we reset it after creating the labels
            var labelWidth = configurationService.LabelWidth;
            var labelHeight = configurationService.LabelHeight;
            var labelSpacing = configurationService.LabelSpacing;

            var label = new Label
            {
                Width = labelWidth,
                Height = labelHeight,
                Content = sequence + 1,
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
            try
            {
                Cursor = Cursors.Wait;

                seriesList.Refresh();

                listBoxSeries.Items.Refresh();
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            seriesList.SaveToDisk();
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            seriesList.ReloadFromDisk();
            listBoxSeries.ItemsSource = seriesList.Series;
        }

        private void GrdEpisodesClick(object sender, RoutedEventArgs e)
        {
            var header = (sender as DataGridColumnHeader)?.Content.ToString();

            switch (header?.ToUpper())
            {
                case "NL":
                    ((Series) listBoxSeries.SelectedItems[0])?.Seasons[ActiveSeason - 1]
                        .ToggleAllSubs(Episode.SubTitle.Nl);
                    break;
                case "EN":
                    ((Series) listBoxSeries.SelectedItems[0])?.Seasons[ActiveSeason - 1]
                        .ToggleAllSubs(Episode.SubTitle.En);
                    break;
                case "DOWNLOADED":
                    ((Series) listBoxSeries.SelectedItems[0])?.Seasons[ActiveSeason - 1].ToggleDownloaded();
                    break;
                case "DATE":
                case "TITLE":
                    break;
                default:
                    Common.Common.Log($"ERROR: Non-coded column header: {header}");
                    break;
            }

            grdEpisodes.Items.Refresh();
        }

        private void btnDebug_Click(object sender, RoutedEventArgs e)
        {
            var series = (Series) listBoxSeries.SelectedItems[0];

            var seasonCompleteness = series.Seasons[ActiveSeason - 1].Episodes
                .Where(c => c.Completeness > CompletenessState.NotApplicable).Select(x => x.Completeness).Max();

            MessageBox.Show(seasonCompleteness.ToString());
        }
    }
}