using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SeriesUI.BusinessLogic;

namespace SeriesUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private int ActiveSeason { get; set; }

        private void OnLabelMouseClick(object sender, EventArgs e)
        {
            // This construct checks for null value
            if (sender is Label label && int.TryParse(label.Content.ToString(), out var newLabelSeason))
            {
                ActiveSeason = newLabelSeason;

                UnMarkSeasonLabels();

                label.Background =
                    (Brush) new BrushConverter().ConvertFrom(ConfigurationManager.AppSettings["LabelActiveColor"]);

                var series = listBoxSeries.SelectedItem as Series;
                if (int.TryParse(label.Content.ToString(), out var season))
                {
                    textBox.Text = "";
                    foreach (var episode in series.Seasons[season - 1].Episodes)
                        textBox.Text += $"{episode.Title}\n";
                }
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
                        (Brush) new BrushConverter().ConvertFrom(
                            ConfigurationManager.AppSettings["LabelInActiveColor"]);
        }

        private void OnLabelMouseEnter(object sender, EventArgs e)
        {
            // This construct checks for null value
            if (sender is Label label && int.TryParse(label.Content.ToString(), out var labelSeason) &&
                labelSeason != ActiveSeason)
                label.Background =
                    (Brush) new BrushConverter().ConvertFrom(ConfigurationManager.AppSettings["LabelActiveColor"]);
        }

        private void OnLabelMouseLeave(object sender, EventArgs e)
        {
            // This construct checks for null value
            if (sender is Label label && int.TryParse(label.Content.ToString(), out var labelSeason) &&
                labelSeason != ActiveSeason)
                label.Background =
                    (Brush) new BrushConverter().ConvertFrom(ConfigurationManager.AppSettings["LabelInActiveColor"]);
        }

        private void listBoxSeries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var labelWidth = double.Parse(ConfigurationManager.AppSettings["LabelWidth"]);
            var labelHeight = double.Parse(ConfigurationManager.AppSettings["labelHeight"]);
            var labelSpacing = double.Parse(ConfigurationManager.AppSettings["labelSpacing"]);

            DeleteSeasonLabels();
            textBox.Text = "";

            var series = (sender as ListBox)?.SelectedItem as Series;

            for (var i = 0; i < series?.Seasons.Count; i++)
            {
                var lbl = new Label
                {
                    Width = labelWidth,
                    Height = labelHeight,
                    Content = i + 1,
                    Background =
                        (Brush) new BrushConverter().ConvertFrom(
                            ConfigurationManager.AppSettings["LabelInActiveColor"]),
                    BorderThickness = new Thickness(0),
                    BorderBrush = Brushes.Black,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(i * (labelWidth + 1) + labelSpacing, 5, 0, 0),
                    Tag = "temporary"
                };

                lbl.MouseEnter += OnLabelMouseEnter;
                lbl.MouseLeave += OnLabelMouseLeave;
                lbl.MouseLeftButtonDown += OnLabelMouseClick;

                grdSeasons.Children.Add(lbl);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            listBoxSeries.Items.Clear();
            listBoxSeries.Items.Refresh();

            try
            {
                Cursor = Cursors.Wait;

                var seriesList = new List<Series>
                {
                    new Series
                    {
                        Name = "The Big Bang Theory",
                        WebSite = "https://www.seriesfeed.com",
                        LocalUrl = "/series/the-big-bang-theory/episodes/season"
                    },
                    new Series
                    {
                        Name = "Modern Family",
                        WebSite = "https://www.seriesfeed.com",
                        LocalUrl = "/series/modern-family/episodes/season"
                    }
                };

                foreach (var series in seriesList)
                {
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