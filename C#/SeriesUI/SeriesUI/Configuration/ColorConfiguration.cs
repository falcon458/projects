using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using SeriesUI.Models.Common;

namespace SeriesUI.Configuration
{
    public class ColorConfiguration : IValueConverter
    {
        public readonly Dictionary<int, Brush> ColorPalette;

        private readonly IConfigurationService configurationService;

        public ColorConfiguration(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;

            ColorPalette = new Dictionary<int, Brush>();

            ConfigurePallette();
        }

        public ColorConfiguration()
        {
            configurationService = new ConfigurationService();

            ColorPalette = new Dictionary<int, Brush>();

            ConfigurePallette();
        }

        /// <summary>
        ///     Used to translate a CompletenessState to a background in a DataGrid
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush result;

            if (value is CompletenessState stateValue)
                result = ColorPalette[new ColorPaletteKey(stateValue, false, typeof(DataGridRow)).GetHashCode()];
            else
                result = (Brush) new BrushConverter().ConvertFrom("White");

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private void ConfigurePallette()
        {
            // Initialize the Color Palette

            // Data rows
            ColorPalette[new ColorPaletteKey(CompletenessState.Complete, true, typeof(DataGridRow)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom("White");
            ColorPalette[new ColorPaletteKey(CompletenessState.Complete, false, typeof(DataGridRow)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom("White");

            ColorPalette[new ColorPaletteKey(CompletenessState.NotSubbedNl, true, typeof(DataGridRow)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotSubbedNlColor);
            ColorPalette[new ColorPaletteKey(CompletenessState.NotSubbedNl, false, typeof(DataGridRow)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotSubbedNlColor);

            ColorPalette[new ColorPaletteKey(CompletenessState.NotSubbed, true, typeof(DataGridRow)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotSubbedColor);
            ColorPalette[new ColorPaletteKey(CompletenessState.NotSubbed, false, typeof(DataGridRow)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotSubbedColor);

            ColorPalette[new ColorPaletteKey(CompletenessState.NotDownloaded, true, typeof(DataGridRow)).GetHashCode()]
                = (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotDownloadedColor);
            ColorPalette[new ColorPaletteKey(CompletenessState.NotDownloaded, false, typeof(DataGridRow)).GetHashCode()]
                = (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotDownloadedColor);

            ColorPalette[new ColorPaletteKey(CompletenessState.NotApplicable, true, typeof(DataGridRow)).GetHashCode()]
                = (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotApplicableColor);
            ColorPalette[new ColorPaletteKey(CompletenessState.NotApplicable, false, typeof(DataGridRow)).GetHashCode()]
                = (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotApplicableColor);

            // Labels
            ColorPalette[new ColorPaletteKey(CompletenessState.Complete, true, typeof(Label)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveCompleteColor);
            ColorPalette[new ColorPaletteKey(CompletenessState.Complete, false, typeof(Label)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveCompleteColor);

            ColorPalette[new ColorPaletteKey(CompletenessState.NotSubbedNl, true, typeof(Label)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotSubbedNlColor);
            ColorPalette[new ColorPaletteKey(CompletenessState.NotSubbedNl, false, typeof(Label)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotSubbedNlColor);

            ColorPalette[new ColorPaletteKey(CompletenessState.NotSubbed, true, typeof(Label)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotSubbedColor);
            ColorPalette[new ColorPaletteKey(CompletenessState.NotSubbed, false, typeof(Label)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotSubbedColor);

            ColorPalette[new ColorPaletteKey(CompletenessState.NotDownloaded, true, typeof(Label)).GetHashCode()]
                = (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotDownloadedColor);
            ColorPalette[new ColorPaletteKey(CompletenessState.NotDownloaded, false, typeof(Label)).GetHashCode()]
                = (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotDownloadedColor);

            ColorPalette[new ColorPaletteKey(CompletenessState.NotApplicable, true, typeof(Label)).GetHashCode()]
                = (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotApplicableColor);
            ColorPalette[new ColorPaletteKey(CompletenessState.NotApplicable, false, typeof(Label)).GetHashCode()]
                = (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotApplicableColor);
        }
    }
}