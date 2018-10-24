using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using SeriesUI.BusinessLogic;
using SeriesUI.Configuration;

// TODO:
// Refresh-merge (equals method?)
// Why do we need to refresh in btnAllNlSubs_Click (and others) to update the grid?
// Colors in grid & labels
// Ask when outstanding unsaved changes (INotifyPropertyChanged)
// Databinding van Series, dan werkt Items.refresh tijdens serie-refresh
// Header-click ipv all-buttons
// Log to file

namespace SeriesUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ConfigurationService configurationService;
        private readonly SeriesList SeriesList;

        public MainWindow()
        {
            InitializeComponent();

            configurationService = new ConfigurationService();

            SeriesList = new SeriesList(configurationService);

            listBoxSeries.ItemsSource = SeriesList.Series;
        }

        // The current season (1-based)
        private int ActiveSeason { get; set; }

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
            try
            {
                Cursor = Cursors.Wait;

                SeriesList.Refresh();

                listBoxSeries.Items.Refresh();
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SeriesList.SaveToDisk();
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            SeriesList.ReloadFromDisk();
            listBoxSeries.ItemsSource = SeriesList.Series;
        }

        private void GrdEpisodesClick(object sender, RoutedEventArgs e)
        {
            var header = (sender as DataGridColumnHeader)?.Content.ToString();

            switch (header?.ToUpper())
            {
                case "NL":
                    ((Series) listBoxSeries.SelectedItems[0])?.Seasons[ActiveSeason - 1]
                        .ToggleAllSubs(Episode.SubTitle.NL);
                    break;
                case "EN":
                    ((Series) listBoxSeries.SelectedItems[0])?.Seasons[ActiveSeason - 1]
                        .ToggleAllSubs(Episode.SubTitle.EN);
                    break;
                case "DOWNLOADED":
                    ((Series) listBoxSeries.SelectedItems[0])?.Seasons[ActiveSeason - 1].ToggleDownloaded();
                    break;
                case "DATE":
                case "TITLE":
                    break;
                default:
                    Common.Log($"ERROR: Non-coded column header: {header}");
                    break;
            }

            grdEpisodes.Items.Refresh();
        }

        private void btnDebug_Click(object sender, RoutedEventArgs e)
        {
            var result = ((Series) listBoxSeries.SelectedItems[0]).Seasons[ActiveSeason - 1].Completeness;

            MessageBox.Show(result.ToString());
        }
    }
}