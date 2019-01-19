using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Catel.Services;
using SeriesUI.BusinessLogic;
using SeriesUI.Configuration;
using SeriesUI.Models.Common;

// TODO:
// Andere kleuren? Omranden? 
// Log to file
// Why do we need to refresh in btnAllNlSubs_Click (and others) to update the grid?
// ColorConfiguration class heeft 2 constructors die beiden worden gebruikt, we hebben dus 2 instances. Kijk of we met 1 af kunnen
// warnings in xaml:  <configuration:ColorConfiguration x:Key="CompletenessToBrushConverter"/>, en anderen
// Zie SetEpisodeEventHandlers: Is dit echt de enige manier om de eventhandler in deze class te krijgen?
// Auto-refresh in background and notify

namespace SeriesUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        // Using a DependencyProperty as the backing store for IsSaved
        public static readonly DependencyProperty IsModifiedProperty =
            DependencyProperty.Register(nameof(IsDataModified), typeof(bool), typeof(MainWindow),
                new PropertyMetadata(false));

        private readonly ColorConfiguration colorConfiguration;
        private readonly IConfigurationService configurationService;
        private readonly SeriesList seriesList;

        public MainWindow()
        {
            InitializeComponent();

            configurationService = new ConfigurationService();

            colorConfiguration = new ColorConfiguration(configurationService);

            seriesList = new SeriesList(configurationService);
            seriesList.SeriesChanged += SeriesListChangeHandler;

            ListBoxSeries.ItemsSource = seriesList.Series;

            ReloadFromDisk();
        }

        public bool IsDataModified
        {
            get => (bool) GetValue(IsModifiedProperty);
            set => SetValue(IsModifiedProperty, value);
        }

        // The current season (1-based)
        private int ActiveSeason { get; set; }

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
                if (ListBoxSeries.SelectedItem is Series series)
                    GrdEpisodes.ItemsSource = series.Seasons[ActiveSeason - 1].Episodes;
            }
        }

        private Brush LabelBackGround(int season, bool active)
        {
            Brush result = null;

            if (ListBoxSeries.SelectedItem is Series series)
            {
                var key = new ColorPaletteKey(series.Seasons[season].Completeness, active, typeof(Label))
                    .GetHashCode();

                result = colorConfiguration.ColorPalette[key];
            }

            return result;
        }

        private void DeleteSeasonLabels()
        {
            foreach (var obj in GrdSeasons.Children.OfType<Label>().Where(c => (c.Tag ?? "").ToString() == "temporary")
                .ToList()) GrdSeasons.Children.Remove(obj);

            ActiveSeason = 0;
        }

        private void UnMarkSeasonLabels()
        {
            foreach (var item in GrdSeasons.Children)
                if (item is Label label && int.TryParse(label.Content.ToString(), out var labelSeason) &&
                    labelSeason != ActiveSeason)
                    label.Background = LabelBackGround(labelSeason - 1, false);
        }

        private void OnLabelMouseEnter(object sender, EventArgs e)
        {
            // [!] This construct checks for null value
            if (sender is Label label)
                SetLabelBackground(label, false);
        }

        private void SetLabelBackground(Label label, bool force)
        {
            if (int.TryParse(label.Content.ToString(), out var labelSeason) && (labelSeason != ActiveSeason || force))
                label.Background = LabelBackGround(labelSeason - 1, true);
        }

        private void OnLabelMouseLeave(object sender, EventArgs e)
        {
            if (sender is Label label && int.TryParse(label.Content.ToString(), out var labelSeason) &&
                labelSeason != ActiveSeason)
                label.Background = LabelBackGround(labelSeason - 1, false);
        }

        private void ListBoxSeries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DeleteSeasonLabels();
            GrdEpisodes.ItemsSource = null;

            var series = (sender as ListBox)?.SelectedItem as Series;

            for (var i = 0; i < series?.Seasons.Count; i++)
            {
                var lbl = GetNewLabel(i);

                GrdSeasons.Children.Add(lbl);
            }

            // Select last season
            if (GrdSeasons.Children.Count > 0)
                OnLabelMouseClick(GrdSeasons.Children[GrdSeasons.Children.Count - 1], new EventArgs());
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

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            // Setting btnSave to "enabled" would break the binding
            var saveEnabled = BindingOperations.GetBindingBase(BtnSave, IsEnabledProperty);

            try
            {
                LabelTotalRefresh.Content = "/" + configurationService.SeriesConfigCollection.Count;
                LabelCurrentRefresh.Content = "0";

                BtnRefresh.IsEnabled = false;
                BtnSave.IsEnabled = false;
                BtnReload.IsEnabled = false;

                Cursor = Cursors.Wait;

                var task = RefreshSeries();
                var dummy = await task;
            }
            finally
            {
                Cursor = Cursors.Arrow;

                BtnRefresh.IsEnabled = true;
                BtnReload.IsEnabled = true;

                BtnSave.SetBinding(IsEnabledProperty, saveEnabled);

                LabelTotalRefresh.Content = "";
                LabelCurrentRefresh.Content = "";
            }
        }

        private async Task<int> RefreshSeries()
        {
            await Task.Run(() => seriesList.Refresh());

            return 1;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            seriesList.SaveToDisk();
            IsDataModified = false;
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            ReloadFromDisk();
        }

        private void ReloadFromDisk()
        {
            seriesList.ReloadFromDisk();
            SetEpisodeEventHandlers();
            ListBoxSeries.ItemsSource = seriesList.Series;
            IsDataModified = false;
        }

        private void GrdEpisodesClick(object sender, RoutedEventArgs e)
        {
            var header = (sender as DataGridColumnHeader)?.Content.ToString();

            switch (header?.ToUpper())
            {
                case "NL":
                    ((Series) ListBoxSeries.SelectedItems[0])?.Seasons[ActiveSeason - 1]
                        .ToggleAllSubs(SubTitle.Nl);
                    break;
                case "EN":
                    ((Series) ListBoxSeries.SelectedItems[0])?.Seasons[ActiveSeason - 1]
                        .ToggleAllSubs(SubTitle.En);
                    break;
                case "DOWNLOADED":
                    ((Series) ListBoxSeries.SelectedItems[0])?.Seasons[ActiveSeason - 1].ToggleDownloaded();
                    break;
                case "DATE":
                case "TITLE":
                    break;
                default:
                    Common.Log($"ERROR: Non-coded column header: {header}");
                    break;
            }

            GrdEpisodes.Items.Refresh();
        }

        private void SetEpisodeEventHandlers(Series series)
        {
            foreach (var season in series.Seasons)
            foreach (var episode in season.Episodes)
                episode.PropertyChanged += EpisodeChangeHandler;
        }

        private void SetEpisodeEventHandlers()
        {
            foreach (var series in seriesList.Series)
            foreach (var season in series.Seasons)
            foreach (var episode in season.Episodes)
                episode.PropertyChanged += EpisodeChangeHandler;
        }

        private void EpisodeChangeHandler(object sender, PropertyChangedEventArgs e)
        {
            GrdEpisodes.Items.Refresh();
            IsDataModified = true;
            SetActiveLabelColor();
            ListBoxSeries.Items.Refresh();
        }

        private void SeriesListChangeHandler(object sender, EventArgs e)
        {
            if (ListBoxSeries.Dispatcher.CheckAccess())
            {
                // We are on the UI thread
                ListBoxSeries.Items.Refresh();

                // Update any new episodes
                if (sender is Series)
                {
                    SetEpisodeEventHandlers(sender as Series);

                    // Use try-parse because this code may conflict with the part where we clear it after refresh
                    if (int.TryParse(LabelCurrentRefresh.Content.ToString(), out var currentNumber))
                        LabelCurrentRefresh.Content = currentNumber + 1;
                }

                IsDataModified = true;
            }
            else
            {
                var formHandler = new UpdateSeriesList(SeriesListChangeHandler);

                // Let the Forms dispatcher handle this call on the UI thread
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, formHandler, sender, e);
            }
        }

        private void SetActiveLabelColor()
        {
            foreach (var item in GrdSeasons.Children)
                if (item is Label label && int.TryParse(label.Content.ToString(), out var labelSeason) &&
                    labelSeason == ActiveSeason)
                    SetLabelBackground(label, true);
        }

        private void BtnDebug_Click(object sender, RoutedEventArgs e)
        {
            foreach (var entryItem in ListBoxSeries.Items)
                if (entryItem is Series)
                {
                    var series = entryItem as Series;

                    // Select the worst completeness
                    Dialog.Show(series.Name + ": " + series.Completeness);
                }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (IsDataModified)
            {
                var reply = Dialog.ShowDialog("Do you want to save the changes?", "Unsaved changes", MessageButton.YesNoCancel, MessageImage.Question);
                switch (reply)
                {
                    case MessageResult.No:
                        break;
                    case MessageResult.Yes:
                        seriesList.SaveToDisk();
                        break;
                    case MessageResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private delegate void UpdateSeriesList(object sender, EventArgs e);
    }
}